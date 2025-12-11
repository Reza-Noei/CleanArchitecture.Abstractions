namespace CleanArchitecture.Mediator.Contracts;

/// <summary>
/// Represents a command that performs a state-changing operation without 
/// returning a meaningful result.
/// </summary>
/// <remarks>
/// This interface is a non-generic specialization of <see cref="ICommand{TResponse}"/> 
/// where the response type is fixed to <see cref="Unit"/>. It is intended for commands 
/// executed solely for their side effects—for example, creating, updating, or deleting 
/// domain entities.
///
/// By standardizing on <see cref="Unit"/> instead of using <c>void</c>, this abstraction 
/// preserves full compatibility with the mediator pipeline model, ensuring that 
/// behaviors such as validation, authorization, logging, and transactions remain 
/// type-safe and uniformly applicable.
/// </remarks>
public interface ICommand : ICommand<Unit>
{
}

/// <summary>
/// Represents a state-changing operation that produces a response value.
/// </summary>
/// <typeparam name="TResponse">
/// The type returned by the command handler upon successful execution.
/// </typeparam>
/// <remarks>
/// Use this generic form of the command contract when an operation produces a result, 
/// such as a newly created identifier, a status object, or any domain-specific value.
///
/// Each command is processed by exactly one command handler and participates in all 
/// configured mediator pipeline behaviors, including logging, validation, authorization, 
/// instrumentation, and transaction scopes. This ensures consistent cross-cutting 
/// concerns across all state-modifying operations.
/// </remarks>
public interface ICommand<TResponse> : IRequest where TResponse : class
{
}
