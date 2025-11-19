
using CleanArchitecture.Specification;
using System.Runtime.InteropServices;

public class SearchEvaluator : IEvaluator
{
    public static SearchEvaluator Instance { get; } = new SearchEvaluator();


    public bool IsCriteriaEvaluator { get; } = true;


    private SearchEvaluator()
    {
    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification is Specification<T> val)
        {
            if (val.OneOrManySearchExpressions.IsEmpty)
            {
                return query;
            }
            SearchExpressionInfo<T> singleOrDefault = val.OneOrManySearchExpressions.SingleOrDefault;
            if (singleOrDefault != null)
            {
                return query.ApplySingleLike(singleOrDefault);
            }
            Span<SearchExpressionInfo<T>> span = CollectionsMarshal.AsSpan(val.OneOrManySearchExpressions.List);
            return ApplyLike(query, (ReadOnlySpan<SearchExpressionInfo<T>>)span);
        }
        foreach (IGrouping<int, SearchExpressionInfo<T>> item in from x in specification.SearchCriterias
                                                                 group x by x.SearchGroup)
        {
            query = query.ApplyLikesAsOrGroup(item);
        }
        return query;
    }

    private static IQueryable<T> ApplyLike<T>(IQueryable<T> source, ReadOnlySpan<SearchExpressionInfo<T>> span) where T : class
    {
        int num = 0;
        for (int i = 1; i <= span.Length; i++)
        {
            if (i == span.Length || span[i].SearchGroup != span[num].SearchGroup)
            {
                IQueryable<T> source2 = source;
                int num2 = num;
                source = source2.ApplyLikesAsOrGroup(span.Slice(num2, i - num2));
                num = i;
            }
        }
        return source;
    }
}
