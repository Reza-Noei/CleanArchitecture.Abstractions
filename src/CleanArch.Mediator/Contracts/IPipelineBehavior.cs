namespace CleanArchitecture.Mediator.Contracts;

/// <summary>
/// Represents the delegate executed by a pipeline behavior or the final request handler.
/// </summary>
/// <typeparam name="TResponse">The type of response produced by the request handler.</typeparam>
/// <param name="cancellationToken">A token to observe while waiting for completion.</param>
/// <returns>
/// A task that completes with the response returned by the next behavior or handler.
/// </returns>
/// <remarks>
/// Pipeline behaviors invoke this delegate to continue the execution flow. 
/// The delegate either calls the next behavior in the chain or, if the 
/// pipeline has ended, invokes the actual request handler.
/// </remarks>
public delegate Task<TResponse> PipelineHandlerDelegate<TResponse>(CancellationToken cancellationToken = default);

/// <summary>
/// Represents the delegate executed by a pipeline behavior or the final handler 
/// for requests that do not produce a result.
/// </summary>
/// <param name="cancellationToken">A token to observe while waiting for completion.</param>
/// <returns>
/// A task representing the asynchronous execution of the next behavior or handler.
/// </returns>
/// <remarks>
/// This non-generic variant is used when the request returns <see cref="Unit"/>, 
/// encapsulating side-effect-only command execution.
/// </remarks>
public delegate Task PipelineHandlerDelegate(CancellationToken cancellationToken = default);


/// <summary>
/// Defines a behavior that wraps the execution of a command or query within the mediator pipeline.
/// </summary>
/// <typeparam name="TRequest">
/// The type of request being processed. Typically <see cref="ICommand"/>, 
/// <see cref="ICommand{TResponse}"/>, or <see cref="IQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of response returned after completing the request handler.
/// </typeparam>
/// <remarks>
/// Pipeline behaviors provide a structured mechanism for applying cross-cutting concerns 
/// such as logging, validation, authorization, caching, metrics, and transactional logic.
/// <para>
/// Behaviors form a chain where each behavior can:
/// <list type="bullet">
/// <item><description>Perform pre-processing logic.</description></item>
/// <item><description>Invoke the next behavior or handler.</description></item>
/// <item><description>Perform post-processing logic.</description></item>
/// </list>
/// </para>
/// <para>
/// A behavior may also short-circuit the pipeline by returning a response without invoking 
/// the next behavior or the final handler.
/// </para>
/// </remarks>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : class
{
    /// <summary>
    /// Executes the behavior and optionally invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">
    /// The next step in the pipeline. Invoking this delegate continues execution 
    /// through remaining behaviors or ultimately the request handler.
    /// </param>
    /// <param name="cancellationToken">A token to observe while waiting for completion.</param>
    /// <returns>
    /// The response produced by the pipeline or final handler.
    /// </returns>
    Task<TResponse> HandleAsync(
        TRequest request,
        PipelineHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}


/// <summary>
/// Defines a behavior that wraps the execution of a command or query that does not produce a result.
/// </summary>
/// <typeparam name="TRequest">
/// The type of request being processed. Typically a non-generic <see cref="ICommand"/>.
/// </typeparam>
/// <remarks>
/// This specialization of <see cref="IPipelineBehavior{TRequest, TResponse}"/> simplifies 
/// pipeline definitions for commands that return <see cref="Unit"/>. 
///
/// Behaviors may perform pre- and post-processing and optionally invoke the next delegate 
/// or short-circuit the pipeline.
/// </remarks>
public interface IPipelineBehavior<in TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : IRequest
{
    /// <summary>
    /// Executes the behavior and optionally invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">The next step in the pipeline.</param>
    /// <param name="cancellationToken">A token to observe while waiting for completion.</param>
    /// <returns>
    /// A task representing the completion of the pipeline or handler.
    /// </returns>
    Task HandleAsync(
        TRequest request,
        PipelineHandlerDelegate next,
        CancellationToken cancellationToken);
}

