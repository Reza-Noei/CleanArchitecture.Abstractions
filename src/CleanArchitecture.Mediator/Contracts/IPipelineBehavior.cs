namespace CleanArchitecture.Mediator.Contracts;

/// <summary>
/// Represents a pipeline behavior that wraps the execution of a command or query.
/// </summary>
/// <typeparam name="TRequest">
/// The type of request being handled. Can be <see cref="ICommand"/>, 
/// <see cref="ICommand{TResponse}"/>, or <see cref="IQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of response returned by the request handler.
/// </typeparam>
/// <remarks>
/// Pipeline behaviors allow you to add cross-cutting concerns around 
/// handlers, such as logging, validation, authorization, caching, 
/// metrics, or transactions.
/// <para>
/// Behaviors form a chain (pipeline) where each behavior can perform 
/// pre-processing, call the next behavior or handler, and then perform 
/// post-processing.
/// </para>
/// <para>
/// Behaviors may also short-circuit the pipeline and return a response 
/// without invoking the actual handler.
/// </para>
/// </remarks>
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
    where TResponse : class
{
    /// <summary>
    /// Executes the pipeline behavior and optionally invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="request">The command or query being processed.</param>
    /// <param name="cancellationToken">A token to observe while waiting for completion.</param>
    /// <param name="next">
    /// The next delegate in the pipeline. Calling this executes the next behavior 
    /// or the final handler.
    /// </param>
    /// <returns>The response produced by the pipeline or final handler.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next);
}



/// <summary>
/// Represents a pipeline behavior that wraps the execution of a command or query.
/// </summary>
/// <typeparam name="TRequest">
/// The type of request being handled. Can be <see cref="ICommand"/>, 
/// <see cref="ICommand{TResponse}"/>, or <see cref="IQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of response returned by the request handler.
/// </typeparam>
/// <remarks>
/// Pipeline behaviors allow you to add cross-cutting concerns around 
/// handlers, such as logging, validation, authorization, caching, 
/// metrics, or transactions.
/// <para>
/// Behaviors form a chain (pipeline) where each behavior can perform 
/// pre-processing, call the next behavior or handler, and then perform 
/// post-processing.
/// </para>
/// <para>
/// Behaviors may also short-circuit the pipeline and return a response 
/// without invoking the actual handler.
/// </para>
/// </remarks>
public interface IPipelineBehavior<TRequest>
    where TRequest : IMessage
{
    /// <summary>
    /// Executes the pipeline behavior and optionally invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="request">The command or query being processed.</param>
    /// <param name="cancellationToken">A token to observe while waiting for completion.</param>
    /// <param name="next">
    /// The next delegate in the pipeline. Calling this executes the next behavior 
    /// or the final handler.
    /// </param>
    /// <returns>The response produced by the pipeline or final handler.</returns>
    Task HandleAsync(TRequest request, CancellationToken cancellationToken, Func<Task> next);
}
