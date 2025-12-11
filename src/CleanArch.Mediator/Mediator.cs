using CleanArchitecture.Mediator.Contracts;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;

namespace CleanArchitecture.Mediator;

public class Mediator : IMediator
{
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _handlers = new ConcurrentDictionary<Type, object>();
        _pipelines = new ConcurrentDictionary<Type, IEnumerable<object>>();
    }

    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerTypeToResolve = typeof(ICommandHandler<>).MakeGenericType(commandType);

        var handler = _handlers.GetOrAdd(commandType, P =>
        {
            var service = _serviceProvider.GetService(handlerTypeToResolve);

            if (service == null)
                throw new InvalidOperationException($"Handler not found for {commandType.Name} (expected {handlerTypeToResolve.FullName}).");

            return service;
        });

        MethodInfo? handleMethod = handlerTypeToResolve.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
        if (handleMethod == null)
            throw new InvalidOperationException($"HandleAsync method not found on handler type {handlerTypeToResolve.FullName}.");

        Func<Task<Unit>> wrappedHandler = BuildHandler<ICommand<Unit>, Unit>(handleMethod, handler, command, cancellationToken);

        await RunPipelineAsync<ICommand, Unit>(wrappedHandler, command, cancellationToken);
    }

    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default) where TResponse : class
    {
        var commandType = command.GetType();
        var responseType = typeof(TResponse);
        var handlerTypeToResolve = typeof(ICommandHandler<,>).MakeGenericType(commandType, responseType);

        var handler = _handlers.GetOrAdd(commandType, P =>
        {
            var service = _serviceProvider.GetService(handlerTypeToResolve);

            if (service == null)
                throw new InvalidOperationException($"Handler not found for {commandType.Name} (expected {handlerTypeToResolve.FullName}).");

            return service;
        });

        MethodInfo? handleMethod = handlerTypeToResolve.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
        if (handleMethod == null)
            throw new InvalidOperationException($"HandleAsync method not found on handler type {handlerTypeToResolve.FullName}.");

        Func<Task<TResponse>> wrappedHandler = BuildHandler<ICommand<TResponse>, TResponse>(handleMethod, handler, command, cancellationToken);

        return await RunPipelineAsync<ICommand<TResponse>, TResponse>(wrappedHandler, command, cancellationToken);
    }

    public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default) where TResponse : class
    {
        var queryType = query.GetType();
        var responseType = typeof(TResponse);
        var handlerTypeToResolve = typeof(IQueryHandler<,>).MakeGenericType(queryType, responseType);

        var handler = _handlers.GetOrAdd(queryType, P =>
        {
            var service = _serviceProvider.GetService(handlerTypeToResolve);

            if (service == null)
                throw new InvalidOperationException($"Handler not found for {queryType.Name} (expected {handlerTypeToResolve.FullName}).");

            return service;
        });

        MethodInfo? handleMethod = handlerTypeToResolve.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
        if (handleMethod == null)
            throw new InvalidOperationException($"HandleAsync method not found on handler type {handlerTypeToResolve.FullName}.");

        Func<Task<TResponse>> wrappedHandler = BuildHandler<IQuery<TResponse>, TResponse>(handleMethod, handler, query, cancellationToken);

        return await RunPipelineAsync<IQuery<TResponse>, TResponse>(wrappedHandler, query, cancellationToken);
    }

    private Func<Task<TResponse>> BuildHandler<TRequest, TResponse>(MethodInfo handleMethod, object handlerInstance, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
        where TResponse : class
    {
        return async () =>
        {
            try
            {
                var taskObj = handleMethod.Invoke(handlerInstance, new object[] { request, cancellationToken }) as Task;
                if (taskObj == null)
                    throw new InvalidOperationException("Handler HandleAsync did not return a Task.");

                await taskObj.ConfigureAwait(false);

                var taskType = taskObj.GetType();
                if (taskType.IsGenericType)
                {
                    PropertyInfo resultProp = taskType.GetProperty("Result")!;
                    return (TResponse?)resultProp!.GetValue(taskObj)!;
                }
                else
                    return default!;
            }
            catch (Exception ex)
            {
                throw ex.InnerException!;
            }
        };
    }

    private IEnumerable<object>? GetPipelineBehaviors(Type requestType, Type responseType)
    {
        var pipelineBehaviorClosedType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var enumerableOfBehaviors = typeof(IEnumerable<>).MakeGenericType(pipelineBehaviorClosedType);
        var behaviorsEnumerableObj = _serviceProvider.GetService(enumerableOfBehaviors)
                                    ?? Array.CreateInstance(pipelineBehaviorClosedType, 0);
        var behaviors = ((IEnumerable<object>)behaviorsEnumerableObj).Cast<object>().ToArray();

        return behaviors;
    }

    private async Task<TResponse> RunPipelineAsync<TRequest, TResponse>(Func<Task<TResponse>> handler, IRequest request, CancellationToken cancellationToken)
        where TResponse : class
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        object[] behaviors = _pipelines.GetOrAdd(requestType, P => GetPipelineBehaviors(requestType, typeof(TResponse)))
            .ToArray();

        var pipelineBehaviorClosedType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        Func<Task<TResponse>> pipeline = handler;
        for (int i = behaviors.Length - 1; i >= 0; i--)
        {
            var behaviorInstance = behaviors[i];

            // The behavior's HandleAsync signature is:
            // Task<TResponse> HandleAsync(TRequest request, CancellationToken ct, Func<Task<TResponse>> next)
            var behaviorHandleMethod = pipelineBehaviorClosedType.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
            if (behaviorHandleMethod == null)
                throw new InvalidOperationException($"HandleAsync not found on behavior type {pipelineBehaviorClosedType.FullName}.");

            // Capture the next delegate for closure
            var nextDelegate = pipeline;

            // Create a delegate compatible with the behavior's expected Func<Task<TResponse>>.
            // Since we're inside a generic method parameterized by TResponse, we can create a strongly-typed lambda:
            Func<Task<TResponse>> nextTyped() => nextDelegate;

            pipeline = () =>
            {
                try
                {
                    // Invoke behavior.HandleAsync(request, cancellationToken, nextDelegate)
                    var resultTaskObj = behaviorHandleMethod.Invoke(behaviorInstance, new object[] { request, cancellationToken, nextDelegate });
                    var resultTask = resultTaskObj as Task;
                    if (resultTask == null)
                        throw new InvalidOperationException("Behavior HandleAsync did not return a Task.");

                    return UnwrapTaskResult<TResponse>(resultTask);
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            };
        }

        // Execute pipeline
        return await pipeline().ConfigureAwait(false);
    }

    /// <summary>
    /// Helper to await a Task and extract its Result if it's Task{T}.
    /// </summary>
    private async Task<T> UnwrapTaskResult<T>(Task task)
    {
        await task.ConfigureAwait(false);
        var taskType = task.GetType();
        if (!taskType.IsGenericType)
            return default!;

        var resultProp = taskType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
        return (T)resultProp!.GetValue(task)!;
    }

    private static ConcurrentDictionary<Type, object> _handlers;
    private static ConcurrentDictionary<Type, IEnumerable<object>> _pipelines;

    private readonly IServiceProvider _serviceProvider;
}
