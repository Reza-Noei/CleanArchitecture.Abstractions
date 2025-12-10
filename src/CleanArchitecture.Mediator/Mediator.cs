using CleanArchitecture.Mediator.Contracts;
using System.Reflection;

namespace CleanArchitecture.Mediator;

/// <summary>
/// Default implementation of the mediator pattern.
/// Routes commands and queries to their corresponding handlers
/// and executes registered pipeline behaviors.
/// </summary>
public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates a new mediator using the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">
    /// The DI service provider used to resolve handlers and pipeline behaviors.
    /// </param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        // ICommand is expected to be ICommand&lt;Unit&gt; under the hood
        return SendInternalAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        return SendInternalAsync<TResponse>(command, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (query == null) throw new ArgumentNullException(nameof(query));
        return SendInternalAsync<TResponse>(query, cancellationToken);
    }

    /// <summary>
    /// Core dispatch path for any request (command/query).
    /// Resolves handler and pipeline behaviors for the concrete request type
    /// and composes the pipeline to execute them in order.
    /// </summary>
    private async Task<TResponse> SendInternalAsync<TResponse>(object request, CancellationToken cancellationToken)
        where TResponse : class
    {
        // Concrete runtime request type (e.g. CreateUserCommand)
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        // Build handler interface type (e.g. ICommandHandler&lt;CreateUserCommand, UserDto&gt;)
        var handlerInterfaceType = GetHandlerInterfaceType(requestType);

        if (handlerInterfaceType == null)
            throw new InvalidOperationException($"Request type {requestType.Name} does not implement ICommand<> or IQuery<>.");

        Type handlerTypeToResolve;
        var genericDef = handlerInterfaceType.GetGenericTypeDefinition();
        if (genericDef == typeof(ICommand<>))
        {
            handlerTypeToResolve = typeof(ICommandHandler<,>).MakeGenericType(requestType, responseType);
        }
        else if (genericDef == typeof(IQuery<>))
        {
            handlerTypeToResolve = typeof(IQueryHandler<,>).MakeGenericType(requestType, responseType);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported request contract for {requestType.Name}");
        }

        // Resolve handler instance
        var handlerInstance = _serviceProvider.GetService(handlerTypeToResolve);
        if (handlerInstance == null)
            throw new InvalidOperationException($"Handler not found for {requestType.Name} (expected {handlerTypeToResolve.FullName}).");

        // Find handler's HandleAsync method (on the handler interface)
        var handleMethod = handlerTypeToResolve.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
        if (handleMethod == null)
            throw new InvalidOperationException($"HandleAsync method not found on handler type {handlerTypeToResolve.FullName}.");

        // Resolve pipeline behaviors: IEnumerable<IPipelineBehavior<requestType, responseType>>
        var pipelineBehaviorClosedType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var enumerableOfBehaviors = typeof(IEnumerable<>).MakeGenericType(pipelineBehaviorClosedType);
        var behaviorsEnumerableObj = _serviceProvider.GetService(enumerableOfBehaviors)
                                    ?? Array.CreateInstance(pipelineBehaviorClosedType, 0);
        var behaviors = ((IEnumerable<object>)behaviorsEnumerableObj).Cast<object>().ToArray();

        // Final handler delegate: Func<Task<TResponse>>
        Func<Task<TResponse>> finalHandler = async () =>
        {
            // Invoke handler.HandleAsync(request, cancellationToken)
            var taskObj = handleMethod.Invoke(handlerInstance, new object[] { request, cancellationToken }) as Task;
            if (taskObj == null)
                throw new InvalidOperationException("Handler HandleAsync did not return a Task.");

            await taskObj.ConfigureAwait(false);

            // If handler returned Task<T>, extract Result, otherwise return default (should not happen for queries/commands with response)
            var taskType = taskObj.GetType();
            if (taskType.IsGenericType)
            {
                var resultProp = taskType.GetProperty("Result");
                return (TResponse?)resultProp!.GetValue(taskObj);
            }

            return default!;
        };

        // Compose behaviors in reverse order so that the first registered behavior is the outermost.
        Func<Task<TResponse>> pipeline = finalHandler;
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
                // Invoke behavior.HandleAsync(request, cancellationToken, nextDelegate)
                var resultTaskObj = behaviorHandleMethod.Invoke(behaviorInstance, new object[] { request, cancellationToken, nextDelegate });
                var resultTask = resultTaskObj as Task;
                if (resultTask == null)
                    throw new InvalidOperationException("Behavior HandleAsync did not return a Task.");

                return UnwrapTaskResult<TResponse>(resultTask);
            };
        }

        // Execute pipeline
        return await pipeline().ConfigureAwait(false);
    }

    /// <summary>
    /// Core dispatch path for any request (command/query).
    /// Resolves handler and pipeline behaviors for the concrete request type
    /// and composes the pipeline to execute them in order.
    /// </summary>
    private async Task SendInternalAsync(ICommand request, CancellationToken cancellationToken)
    {
        // Concrete runtime request type (e.g. CreateUserCommand)
        var requestType = request.GetType();

        // Build handler interface type (e.g. ICommandHandler&lt;CreateUserCommand, UserDto&gt;)
        var handlerInterfaceType = GetHandlerInterfaceTypeForNonGenericCommand(requestType);

        if (handlerInterfaceType == null)
            throw new InvalidOperationException($"Request type {requestType.Name} does not implement ICommand<> or IQuery<>.");

        Type handlerTypeToResolve;

        if (handlerInterfaceType == typeof(ICommand))
        {
            handlerTypeToResolve = typeof(ICommandHandler<>).MakeGenericType(requestType);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported request contract for {requestType.Name}");
        }

        // Resolve handler instance
        var handlerInstance = _serviceProvider.GetService(handlerTypeToResolve);
        if (handlerInstance == null)
            throw new InvalidOperationException($"Handler not found for {requestType.Name} (expected {handlerTypeToResolve.FullName}).");

        // Find handler's HandleAsync method (on the handler interface)
        var handleMethod = handlerTypeToResolve.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
        if (handleMethod == null)
            throw new InvalidOperationException($"HandleAsync method not found on handler type {handlerTypeToResolve.FullName}.");

        // Resolve pipeline behaviors: IEnumerable<IPipelineBehavior<requestType, responseType>>
        var pipelineBehaviorClosedType = typeof(IPipelineBehavior<>).MakeGenericType(requestType);
        var enumerableOfBehaviors = typeof(IEnumerable<>).MakeGenericType(pipelineBehaviorClosedType);
        var behaviorsEnumerableObj = _serviceProvider.GetService(enumerableOfBehaviors)
                                    ?? Array.CreateInstance(pipelineBehaviorClosedType, 0);
        var behaviors = ((IEnumerable<object>)behaviorsEnumerableObj).Cast<object>().ToArray();

        // Final handler delegate: Func<Task<TResponse>>
        Func<Task> finalHandler = async () =>
        {
            try
            {
                var taskObj = handleMethod.Invoke(handlerInstance, new object[] { request, cancellationToken }) as Task;
                if (taskObj == null)
                    throw new InvalidOperationException("Handler HandleAsync did not return a Task.");

                await taskObj.ConfigureAwait(false);

                // If handler returned Task<T>, extract Result, otherwise return default (should not happen for queries/commands with response)
                var taskType = taskObj.GetType();
                if (taskType.IsGenericType)
                {
                    var resultProp = taskType.GetProperty("Result");
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException!;
            }
        };

        // Compose behaviors in reverse order so that the first registered behavior is the outermost.
        Func<Task> pipeline = finalHandler;
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
            Func<Task> nextTyped() => nextDelegate;

            pipeline = () =>
            {
                // Invoke behavior.HandleAsync(request, cancellationToken, nextDelegate)
                var resultTaskObj = behaviorHandleMethod.Invoke(behaviorInstance, new object[] { request, cancellationToken, nextDelegate });
                var resultTask = resultTaskObj as Task;
                if (resultTask == null)
                    throw new InvalidOperationException("Behavior HandleAsync did not return a Task.");

                return resultTask;
            };
        }

        // Execute pipeline
        await pipeline().ConfigureAwait(false);
    }

    /// <summary>
    /// Helper to await a Task and extract its Result if it's Task{T}.
    /// </summary>
    private static async Task<T> UnwrapTaskResult<T>(Task task)
    {
        await task.ConfigureAwait(false);
        var taskType = task.GetType();
        if (!taskType.IsGenericType)
            return default!;

        var resultProp = taskType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
        return (T)resultProp!.GetValue(task)!;
    }

    /// <summary>
    /// Finds the implemented IQuery&lt;T&gt; or ICommand&lt;T&gt; interface on the request type.
    /// Returns the closed generic interface type (e.g. ICommand{UserDto}).
    /// </summary>
    private static Type? GetHandlerInterfaceType(Type requestType)
    {
        // Inspect all interfaces implemented by the request type and find ICommand<> or IQuery<>
        var iface = requestType.GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType &&
                (i.GetGenericTypeDefinition() == typeof(ICommand<>) ||
                 i.GetGenericTypeDefinition() == typeof(IQuery<>)));

        return iface;
    }

    /// <summary>
    /// Finds the implemented IQuery&lt;T&gt; or ICommand&lt;T&gt; interface on the request type.
    /// Returns the closed generic interface type (e.g. ICommand{UserDto}).
    /// </summary>
    private static Type? GetHandlerInterfaceTypeForNonGenericCommand(Type requestType)
    {
        // Inspect all interfaces implemented by the request type and find ICommand<> or IQuery<>
        var iface = requestType.GetInterfaces()
            .FirstOrDefault(P => P == typeof(ICommand));
        return iface;
    }
}