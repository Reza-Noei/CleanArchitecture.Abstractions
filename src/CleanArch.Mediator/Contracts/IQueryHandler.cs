namespace CleanArchitecture.Mediator.Contracts;

/// <summary>
/// Defines a handler responsible for processing a specific <see cref="IQuery{TResponse}"/>.
/// </summary>
/// <typeparam name="TQuery">
/// The type of query this handler processes.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of response returned by the query handler.
/// </typeparam>
/// <remarks>
/// A query handler encapsulates the read-side logic in a CQRS architecture. 
/// It is responsible for retrieving data from the system without modifying state 
/// and should remain side-effect free.
///
/// Each query is expected to have exactly one corresponding handler. Pipeline behaviors 
/// (such as logging, caching, validation, authorization, or metrics) may be applied 
/// around the execution of this handler to provide cross-cutting functionality.
/// </remarks>
public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : class
{
    /// <summary>
    /// Executes the query and returns the requested data.
    /// </summary>
    /// <param name="query">The query request containing criteria for data retrieval.</param>
    /// <param name="cancellationToken">A token to observe while waiting for completion or for cancellation.</param>
    /// <returns>
    /// A task that completes with the result of the query of type <typeparamref name="TResponse"/>.
    /// </returns>
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
