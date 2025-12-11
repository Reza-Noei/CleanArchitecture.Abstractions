namespace CleanArchitecture.Mediator.Contracts;

/// <summary>
/// Represents the central dispatching component for application requests.
/// </summary>
/// <remarks>
/// The mediator serves as a single entry point for sending <see cref="ICommand"/> 
/// and <see cref="IQuery{TResponse}"/> objects. It resolves the corresponding handler 
/// and executes any registered pipeline behaviors.
///
/// This abstraction supports a clean separation of concerns by decoupling request 
/// issuers from their handlers and promoting a consistent, CQRS-oriented architecture.
/// </remarks>
public interface IMediator
{
    /// <summary>
    /// Dispatches a command that does not produce a response value.
    /// </summary>
    /// <remarks>
    /// Use this method for commands executed purely for their side effects, such as:
    /// <list type="bullet">
    /// <item><description>CreateOrderCommand</description></item>
    /// <item><description>DeleteUserCommand</description></item>
    /// <item><description>PublishEventCommand</description></item>
    /// </list>
    ///
    /// The operation completes successfully if no exception is thrown.
    /// </remarks>
    /// <param name="command">The command to be dispatched.</param>
    /// <param name="cancellationToken">A token used to observe cancellation requests.</param>
    Task SendAsync(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a command that produces a response value.
    /// </summary>
    /// <typeparam name="TResponse">The type of the result returned by the handler.</typeparam>
    /// <param name="command">The command to be dispatched.</param>
    /// <param name="cancellationToken">A token used to observe cancellation requests.</param>
    /// <returns>
    /// A task that completes with the response produced by the command handler.
    /// </returns>
    /// <remarks>
    /// Use this method when command execution must return data, such as:
    /// <list type="bullet">
    /// <item><description>RegisterUserCommand → UserDto</description></item>
    /// <item><description>CreateInvoiceCommand → Guid</description></item>
    /// <item><description>GenerateTokenCommand → JwtToken</description></item>
    /// </list>
    /// </remarks>
    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        where TResponse : class;

    /// <summary>
    /// Dispatches a query that retrieves data and returns a response value.
    /// </summary>
    /// <typeparam name="TResponse">The type of the data returned by the query handler.</typeparam>
    /// <param name="query">The query to be dispatched.</param>
    /// <param name="cancellationToken">A token used to observe cancellation requests.</param>
    /// <returns>
    /// A task that completes with the response produced by the query handler.
    /// </returns>
    /// <remarks>
    /// Queries must be idempotent and free of side effects. Common examples include:
    /// <list type="bullet">
    /// <item><description>GetOrderByIdQuery → OrderDto</description></item>
    /// <item><description>SearchProductsQuery → ProductListDto</description></item>
    /// <item><description>GetUserProfileQuery → UserProfileDto</description></item>
    /// </list>
    /// </remarks>
    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        where TResponse : class;
}
