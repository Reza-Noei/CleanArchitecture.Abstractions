using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;

public sealed class IncludeStringEvaluator : IEvaluator
{
    public static IncludeStringEvaluator Instance { get; } = new IncludeStringEvaluator();


    public bool IsCriteriaEvaluator => false;

    private IncludeStringEvaluator()
    {
    }

    /// <inheritdoc />
    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification is Specification<T> val)
        {
            if (val.OneOrManyIncludeStrings.IsEmpty)
            {
                return query;
            }
            string singleOrDefault = val.OneOrManyIncludeStrings.SingleOrDefault;
            if (singleOrDefault != null)
            {
                return EntityFrameworkQueryableExtensions.Include<T>(query, singleOrDefault);
            }
            {
                foreach (string item in val.OneOrManyIncludeStrings.List)
                {
                    query = EntityFrameworkQueryableExtensions.Include<T>(query, item);
                }
                return query;
            }
        }
        foreach (string includeString in specification.IncludeStrings)
        {
            query = EntityFrameworkQueryableExtensions.Include<T>(query, includeString);
        }
        return query;
    }
}