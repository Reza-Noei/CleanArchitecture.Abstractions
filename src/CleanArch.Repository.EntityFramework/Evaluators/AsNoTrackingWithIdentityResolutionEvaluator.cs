using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;

public class AsNoTrackingWithIdentityResolutionEvaluator : IEvaluator
{
    public static AsNoTrackingWithIdentityResolutionEvaluator Instance { get; } = new AsNoTrackingWithIdentityResolutionEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;

    private AsNoTrackingWithIdentityResolutionEvaluator()
    {

    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.AsNoTrackingWithIdentityResolution)
        {
            query = EntityFrameworkQueryableExtensions.AsNoTrackingWithIdentityResolution<T>(query);
        }

        return query;
    }
}
