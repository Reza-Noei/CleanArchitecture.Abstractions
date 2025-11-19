using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;

public class AsSplitQueryEvaluator : IEvaluator
{
    public static AsSplitQueryEvaluator Instance { get; } = new AsSplitQueryEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;

    private AsSplitQueryEvaluator()
    {

    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.AsSplitQuery)
        {
            query = RelationalQueryableExtensions.AsSplitQuery<T>(query);
        }

        return query;
    }
}
