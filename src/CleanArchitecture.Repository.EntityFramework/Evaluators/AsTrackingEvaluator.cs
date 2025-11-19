using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;

public class AsTrackingEvaluator : IEvaluator
{
    public static AsTrackingEvaluator Instance { get; } = new AsTrackingEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;

    private AsTrackingEvaluator()
    {

    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.AsTracking)
        {
            query = EntityFrameworkQueryableExtensions.AsTracking<T>(query);
        }
        return query;
    }
}
