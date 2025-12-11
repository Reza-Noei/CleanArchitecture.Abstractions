namespace CleanArchitecture.Mediator.Contracts;

/// <summary>
/// Represents a read-only request that retrieves data from the system.
/// </summary>
/// <typeparam name="TResponse">
/// The type of data returned by the query handler.
/// </typeparam>
/// <remarks>
/// Queries belong to the read side of a CQRS architecture and must never modify 
/// application state. Their purpose is to retrieve, project, or filter data, 
/// typically optimized for read performance and efficiency.
///
/// Each query is processed by exactly one <see cref="IQueryHandler{TQuery, TResponse}"/> 
/// implementation and is expected to be idempotent and side-effect free.
///
/// This abstraction enables consistent mediator dispatching and integration with 
/// cross-cutting behaviors such as caching, logging, authorization, or metrics.
/// </remarks>
public interface IQuery<TResponse> : IRequest where TResponse : class
{

}