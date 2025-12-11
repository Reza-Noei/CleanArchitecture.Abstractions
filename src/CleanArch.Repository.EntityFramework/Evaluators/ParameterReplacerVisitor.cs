
using System.Linq.Expressions;

public sealed class ParameterReplacerVisitor : ExpressionVisitor
{
    private ParameterExpression _oldParameter;

    private Expression _newExpression;

    public ParameterReplacerVisitor(ParameterExpression oldParameter, Expression newExpression)
    {
        _oldParameter = oldParameter;
        _newExpression = newExpression;
    }

    internal void Update(ParameterExpression oldParameter, Expression newExpression)
    {
        _oldParameter = oldParameter;
        _newExpression = newExpression;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (node != _oldParameter)
        {
            return node;
        }
        return _newExpression;
    }
}
