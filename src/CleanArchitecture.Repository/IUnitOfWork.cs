namespace CleanArchitecture.Repository;

/// <summary>
/// Defines a unit of work that coordinates the execution of database operations
/// within a single logical transaction. This interface provides explicit control
/// over transactional boundaries and commit behavior, ensuring that all changes
/// are persisted atomically.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all changes tracked by the underlying database context.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.  
    /// The task result contains the number of state entries written to the database.
    /// </returns>
    /// <remarks>
    /// This method does not begin or commit a database transaction by itself.
    /// If a transaction is required, call <see cref="BeginTransactionAsync"/> first.
    /// </remarks>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
