using CleanArchitecture.Specification;

using System.Runtime.InteropServices;

/// <inheritdoc />
public class SpecificationEvaluator : ISpecificationEvaluator
{
    /// <summary>
    /// <see cref="T:Ardalis.Specification.EntityFrameworkCore.SpecificationEvaluator" /> instance with default evaluators..
    /// </summary>
    public static SpecificationEvaluator Default { get; } = new SpecificationEvaluator();


    protected List<IEvaluator> Evaluators { get; }

    public SpecificationEvaluator()
    {
        int num = 13;
        List<IEvaluator> list = new List<IEvaluator>(num);
        CollectionsMarshal.SetCount(list, num);
        Span<IEvaluator> span = CollectionsMarshal.AsSpan(list);
        int num2 = 0;
        span[num2] = (IEvaluator)(object)WhereEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)SearchEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)IncludeStringEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)IncludeEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)OrderEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)PaginationEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)AsNoTrackingEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)AsNoTrackingWithIdentityResolutionEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)AsTrackingEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)IgnoreQueryFiltersEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)IgnoreAutoIncludesEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)AsSplitQueryEvaluator.Instance;
        num2++;
        span[num2] = (IEvaluator)(object)QueryTagEvaluator.Instance;
        Evaluators = list;
    }

    public SpecificationEvaluator(IEnumerable<IEvaluator> evaluators)
    {
        Evaluators = evaluators.ToList();
    }

    /// <inheritdoc />
    public virtual IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification) where T : class
    {
        ArgumentNullException.ThrowIfNull(specification, "specification");

        if (specification.Selector == null && specification.SelectorMany == null)
        {
            throw new SelectorNotFoundException();
        }
        if (specification.Selector != null && specification.SelectorMany != null)
        {
            throw new ConcurrentSelectorsException();
        }

        query = GetQuery(query, (ISpecification<T>)(object)specification);
        if (specification.Selector == null)
        {
            return query.SelectMany(specification.SelectorMany);
        }
        
        return query.Select(specification.Selector);
    }

    /// <inheritdoc />
    public virtual IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification, bool evaluateCriteriaOnly = false) where T : class
    {
        ArgumentNullException.ThrowIfNull(specification, "specification");

        IEnumerable<IEvaluator> enumerable;
        
        if (!evaluateCriteriaOnly)
        {
            IEnumerable<IEvaluator> evaluators = Evaluators;
            enumerable = evaluators;
        }
        else
        {
            enumerable = Evaluators.Where((IEvaluator x) => x.IsCriteriaEvaluator);
        }
        foreach (IEvaluator item in enumerable)
        {
            query = item.GetQuery<T>(query, specification);
        }
        return query;
    }
}
