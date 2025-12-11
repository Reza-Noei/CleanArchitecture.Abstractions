using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;

public class QueryTagEvaluator : IEvaluator
{
    public static QueryTagEvaluator Instance { get; } = new QueryTagEvaluator();


    public bool IsCriteriaEvaluator { get; } = true;


    private QueryTagEvaluator()
    {
    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification is Specification<T> val)
        {
            if (val.OneOrManyQueryTags.IsEmpty)
            {
                return query;
            }
            if (val.OneOrManyQueryTags.HasSingleItem)
            {
                return EntityFrameworkQueryableExtensions.TagWith<T>(query, val.OneOrManyQueryTags.Single);
            }
            {
                foreach (string item in val.OneOrManyQueryTags.List)
                {
                    query = EntityFrameworkQueryableExtensions.TagWith<T>(query, item);
                }
                return query;
            }
        }
        foreach (string queryTag in specification.QueryTags)
        {
            query = EntityFrameworkQueryableExtensions.TagWith<T>(query, queryTag);
        }
        return query;
    }
}
