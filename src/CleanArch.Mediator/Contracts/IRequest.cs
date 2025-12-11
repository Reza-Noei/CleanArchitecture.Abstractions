namespace CleanArchitecture.Mediator.Contracts;

/// <summary>
/// Serves as the fundamental contract for all request messages processed by the Mediator.
/// </summary>
/// <remarks>
/// This marker interface provides a unified abstraction for commands, queries, and any 
/// other request types participating in the Mediator pipeline. Although it does not 
/// declare members, its presence enables the framework to treat all request messages 
/// uniformly.
///
/// Implementations of <see cref="IRequest"/> can participate in cross-cutting behaviors 
/// such as validation, authorization, instrumentation, logging, and transaction pipelines.
/// This design facilitates a clean separation between application logic and infrastructure 
/// concerns, aligning with CQRS and Mediator architectural principles.
/// </remarks>
public interface IRequest
{

}
