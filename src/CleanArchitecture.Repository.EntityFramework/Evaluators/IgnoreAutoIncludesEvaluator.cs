using CleanArchitecture.Specification;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// This evaluator applies EF Core's IgnoreAutoIncludes to a given query
/// </summary>
public class IgnoreAutoIncludesEvaluator : IEvaluator
{
    public static IgnoreAutoIncludesEvaluator Instance { get; } = new IgnoreAutoIncludesEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;

    private IgnoreAutoIncludesEvaluator()
    {

    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.IgnoreAutoIncludes)
        {
            query = EntityFrameworkQueryableExtensions.IgnoreAutoIncludes<T>(query);
        }
        return query;
    }
}
