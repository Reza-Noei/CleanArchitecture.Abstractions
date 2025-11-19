namespace CleanArchitecture.Specification;

public class BaseSpecification<T>
{
    protected Expression<Func<T, bool>> _criteria;

    public Expression<Func<T, bool>> Criteria => _criteria;

    public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

    public List<string> IncludeStrings { get; } = new List<string>();

    public Expression<Func<T, object>> OrderBy { get; private set; }

    public Expression<Func<T, object>> OrderByDescending { get; private set; }

    public int Take { get; private set; }

    public int Skip { get; private set; }

    public bool isPagingEnabled { get; private set; }

    public string CacheKey { get; protected set; }

    public bool CacheEnabled { get; private set; }

    public void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    public BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        _criteria = criteria;
    }

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        isPagingEnabled = true;
    }

    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    protected void EnableCache(string specificationName, params object[] args)
    {
        CacheKey = specificationName + "-" + string.Join("-", args);
        CacheEnabled = true;
    }
}