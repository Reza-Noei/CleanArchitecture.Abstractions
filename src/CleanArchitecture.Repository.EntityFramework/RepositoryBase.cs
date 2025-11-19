using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Repository.EntityFramework;

public abstract class RepositoryBase<T> : IRepositoryBase<T>, IReadRepositoryBase<T> where T : class
{
    protected DbContext DbContext { get; set; }

    protected ISpecificationEvaluator SpecificationEvaluator { get; set; }

    public RepositoryBase(DbContext dbContext): this(dbContext, EntityFramework.SpecificationEvaluator.Default)
    {
    }

    public RepositoryBase(DbContext dbContext, ISpecificationEvaluator specificationEvaluator)
    {
        DbContext = dbContext;
        SpecificationEvaluator = specificationEvaluator;
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        DbContext.Set<T>().Add(entity);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        DbContext.Set<T>().AddRange(entities);
        await SaveChangesAsync(cancellationToken);
        return entities;
    }

    public virtual async Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        DbContext.Set<T>().Update(entity);
        return await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        DbContext.Set<T>().UpdateRange(entities);
        return await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        DbContext.Set<T>().Remove(entity);
        return await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        DbContext.Set<T>().RemoveRange(entities);
        return await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> DeleteRangeAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        IQueryable<T> entities = ApplySpecification(specification);
        DbContext.Set<T>().RemoveRange(entities);
        return await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default(CancellationToken)) where TId : notnull
    {
        return await DbContext.Set<T>().FindAsync(new object[1] { id }, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<List<T>> ListAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await DbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        List<T> list = await ApplySpecification(specification).ToListAsync(cancellationToken);
        return (specification.PostProcessingAction == null) ? list : specification.PostProcessingAction(list).AsList();
    }

    public virtual async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        List<TResult> list = await ApplySpecification(specification).ToListAsync(cancellationToken);
        return (specification.PostProcessingAction == null) ? list : specification.PostProcessingAction(list).AsList();
    }

    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await ApplySpecification(specification, evaluateCriteriaOnly: true).CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await DbContext.Set<T>().CountAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await ApplySpecification(specification, evaluateCriteriaOnly: true).AnyAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await DbContext.Set<T>().AnyAsync(cancellationToken);
    }

    public virtual IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
    {
        return ApplySpecification(specification).AsAsyncEnumerable();
    }

    //
    // Summary:
    //     Filters the entities of T, to those that match the encapsulated query logic of
    //     the specification.
    //
    // Parameters:
    //   specification:
    //     The encapsulated query logic.
    //
    //   evaluateCriteriaOnly:
    //     It ignores pagination and evaluators that don't affect Count.
    //
    // Returns:
    //     The filtered entities as an System.Linq.IQueryable`1.
    protected virtual IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
    {
        return SpecificationEvaluator.GetQuery(DbContext.Set<T>().AsQueryable(), specification, evaluateCriteriaOnly);
    }

    //
    // Summary:
    //     Filters all entities of T, that matches the encapsulated query logic of the specification,
    //     from the database.
    //
    //     Projects each entity into a new form, being TResult.
    //
    // Parameters:
    //   specification:
    //     The encapsulated query logic.
    //
    // Type parameters:
    //   TResult:
    //     The type of the value returned by the projection.
    //
    // Returns:
    //     The filtered projected entities as an System.Linq.IQueryable`1.
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
    {
        return SpecificationEvaluator.GetQuery(DbContext.Set<T>().AsQueryable(), specification);
    }
}
