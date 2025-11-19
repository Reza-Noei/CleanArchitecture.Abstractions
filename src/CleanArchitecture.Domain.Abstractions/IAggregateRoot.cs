using CleanArchitecture.Specification;

namespace CleanArchitecture.Domain.Abstractions;

/// <summary>
/// Apply this marker interface only to aggregate root entities in your domain model
/// Your repository implementation can use constraints to ensure it only operates on aggregate roots
/// </summary>
public interface IAggregateRoot { }

/// <summary>
/// Represents a specification with a selector for projecting entities of type <typeparamref name="T"/> to <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="T">The type being queried against.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface ISpecification<T, TResult> : ISpecification<T>
{
    /// <summary>
    /// Gets the specification builder.
    /// </summary>
    new ISpecificationBuilder<T, TResult> Query { get; }

    /// <summary>
    /// The Select transform function to apply to the <typeparamref name="T"/> element.
    /// </summary>
    Expression<Func<T, TResult>>? Selector { get; }

    /// <summary>
    /// The SelectMany transform function to apply to the <typeparamref name="T"/> element.
    /// </summary>
    Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; }

    /// <summary>
    /// The transform function to apply to the result of the query encapsulated by the <see cref="ISpecification{T, TResult}"/>.
    /// </summary>
    new Func<IEnumerable<TResult>, IEnumerable<TResult>>? PostProcessingAction { get; }

    /// <summary>
    /// Applies the specification to the given entities and returns the filtered results.
    /// </summary>
    /// <param name="entities">The entities to evaluate.</param>
    /// <returns>The filtered results after applying the specification.</returns>
    new IEnumerable<TResult> Evaluate(IEnumerable<T> entities);
}
