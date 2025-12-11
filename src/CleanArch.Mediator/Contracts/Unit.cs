namespace CleanArchitecture.Mediator.Contracts;

/// <summary>
/// Represents a void or “nothing” result in a generic, type-safe way.
/// </summary>
/// <remarks>
/// The <see cref="Unit"/> type is used as a placeholder for commands or queries
/// that do not return any meaningful value. It allows pipeline behaviors,
/// mediators, and handlers to remain generic and type-safe even when no
/// response is needed.
///
/// This pattern is inspired by functional programming and is widely used in
/// CQRS libraries such as MediatR.
/// </remarks>
public sealed class Unit
{
    /// <summary>
    /// The singleton instance of <see cref="Unit"/>.
    /// </summary>
    public static readonly Unit Value = new();

    /// <summary>
    /// Private constructor to prevent external instantiation.
    /// </summary>
    private Unit() { }
}