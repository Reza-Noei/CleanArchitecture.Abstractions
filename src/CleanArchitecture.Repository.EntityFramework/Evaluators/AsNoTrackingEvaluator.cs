using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;

public class AsNoTrackingEvaluator : IEvaluator
{
    public static AsNoTrackingEvaluator Instance { get; } = new AsNoTrackingEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;

    private AsNoTrackingEvaluator()
    {

    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.AsNoTracking)
        {
            query = EntityFrameworkQueryableExtensions.AsNoTracking<T>(query);
        }
        return query;
    }
}
