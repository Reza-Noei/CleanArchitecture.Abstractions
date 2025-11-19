using CleanArchitecture.Specification;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// This evaluator applies EF Core's IgnoreQueryFilters feature to a given query
/// See: https://docs.microsoft.com/en-us/ef/core/querying/filters
/// </summary>
public class IgnoreQueryFiltersEvaluator : IEvaluator
{
    public static IgnoreQueryFiltersEvaluator Instance { get; } = new IgnoreQueryFiltersEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;

    private IgnoreQueryFiltersEvaluator()
    {

    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.IgnoreQueryFilters)
        {
            query = EntityFrameworkQueryableExtensions.IgnoreQueryFilters<T>(query);
        }

        return query;
    }
}
