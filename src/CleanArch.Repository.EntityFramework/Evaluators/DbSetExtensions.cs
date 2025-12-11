using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;
using CollectionExtensions = CleanArchitecture.Specification.CollectionExtensions;

public static class DbSetExtensions
{
    public static async Task<List<TSource>> ToListAsync<TSource>(this DbSet<TSource> source, ISpecification<TSource> specification, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class
    {
        List<TSource> list = await EntityFrameworkQueryableExtensions.ToListAsync<TSource>(SpecificationEvaluator.Default.GetQuery((IQueryable<TSource>)source, specification), cancellationToken);
        
        return (specification.PostProcessingAction == null) ? list : CollectionExtensions.AsList<TSource>(specification.PostProcessingAction(list));
    }

    public static async Task<IEnumerable<TSource>> ToEnumerableAsync<TSource>(this DbSet<TSource> source, ISpecification<TSource> specification, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class
    {
        List<TSource> list = await EntityFrameworkQueryableExtensions.ToListAsync<TSource>(SpecificationEvaluator.Default.GetQuery((IQueryable<TSource>)source, specification), cancellationToken);
        
        IEnumerable<TSource> result;
        
        if (specification.PostProcessingAction != null)
        {
            result = specification.PostProcessingAction(list);
        }
        else
        {
            IEnumerable<TSource> enumerable = list;
            result = enumerable;
        }

        return result;
    }

    public static IQueryable<TSource> WithSpecification<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification, ISpecificationEvaluator? evaluator = null) where TSource : class
    {
        if (evaluator == null)
        {
            evaluator = (ISpecificationEvaluator?)(object)SpecificationEvaluator.Default;
        }
        
        return evaluator!.GetQuery<TSource>(source, specification, false);
    }

    public static IQueryable<TResult> WithSpecification<TSource, TResult>(this IQueryable<TSource> source, ISpecification<TSource, TResult> specification, ISpecificationEvaluator? evaluator = null) where TSource : class
    {
        if (evaluator == null)
        {
            evaluator = (ISpecificationEvaluator?)(object)SpecificationEvaluator.Default;
        }
        
        return evaluator!.GetQuery<TSource, TResult>(source, specification);
    }
}
