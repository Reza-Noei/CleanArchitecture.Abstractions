
using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

public static class SearchExtension
{
    private record StringVar(string Format)
    {
        [CompilerGenerated]
        protected virtual Type EqualityContract
        {
            [CompilerGenerated]
            get
            {
                return typeof(StringVar);
            }
        }

        [CompilerGenerated]
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("StringVar");
            stringBuilder.Append(" { ");
            if (PrintMembers(stringBuilder))
            {
                stringBuilder.Append(' ');
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString();
        }

        [CompilerGenerated]
        protected virtual bool PrintMembers(StringBuilder builder)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            builder.Append("Format = ");
            builder.Append((object?)Format);
            return true;
        }

        [CompilerGenerated]
        public override int GetHashCode()
        {
            return EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Format);
        }

        [CompilerGenerated]
        public virtual bool Equals(StringVar? other)
        {
            if ((object)this != other)
            {
                if ((object)other != null && EqualityContract == other!.EqualityContract)
                {
                    return EqualityComparer<string>.Default.Equals(Format, other!.Format);
                }
                return false;
            }
            return true;
        }

        [CompilerGenerated]
        protected StringVar(StringVar original)
        {
            Format = original.Format;
        }
    }

    private static readonly PropertyInfo _stringFormatProperty = typeof(StringVar)!.GetProperty("Format");

    private static readonly MemberExpression _functions = Expression.Property(null, typeof(EF)!.GetProperty("Functions"));

    private static readonly MethodInfo _likeMethodInfo = typeof(DbFunctionsExtensions)!.GetMethod("Like", new Type[3]
    {
        typeof(DbFunctions),
        typeof(string),
        typeof(string)
    });

    private static MemberExpression StringAsExpression(string value)
    {
        return Expression.Property(Expression.Constant(new StringVar(value)), _stringFormatProperty);
    }

    public static IQueryable<T> ApplySingleLike<T>(this IQueryable<T> source, SearchExpressionInfo<T> searchExpression)
    {
        ParameterExpression parameterExpression = searchExpression.Selector.Parameters[0];
        Expression body = searchExpression.Selector.Body;
        MemberExpression arg = StringAsExpression(searchExpression.SearchTerm);
        MethodCallExpression body2 = Expression.Call(null, _likeMethodInfo, _functions, body, arg);
        return source.Where(Expression.Lambda<Func<T, bool>>(body2, new ParameterExpression[1] { parameterExpression }));
    }

    [OverloadResolutionPriority(1)]
    public static IQueryable<T> ApplyLikesAsOrGroup<T>(this IQueryable<T> source, ReadOnlySpan<SearchExpressionInfo<T>> searchExpressions)
    {
        Expression combinedExpr = null;
        ParameterExpression mainParam = null;
        ParameterReplacerVisitor visitor = null;
        ReadOnlySpan<SearchExpressionInfo<T>> readOnlySpan = searchExpressions;
        for (int i = 0; i < readOnlySpan.Length; i++)
        {
            SearchExpressionInfo<T> searchExpression = readOnlySpan[i];
            ApplyLikeAsOrGroup<T>(ref combinedExpr, ref mainParam, ref visitor, searchExpression);
        }
        if (combinedExpr != null && mainParam != null)
        {
            return source.Where(Expression.Lambda<Func<T, bool>>(combinedExpr, new ParameterExpression[1] { mainParam }));
        }
        return source;
    }

    public static IQueryable<T> ApplyLikesAsOrGroup<T>(this IQueryable<T> source, IEnumerable<SearchExpressionInfo<T>> searchExpressions)
    {
        Expression combinedExpr = null;
        ParameterExpression mainParam = null;
        ParameterReplacerVisitor visitor = null;
        foreach (SearchExpressionInfo<T> searchExpression in searchExpressions)
        {
            ApplyLikeAsOrGroup<T>(ref combinedExpr, ref mainParam, ref visitor, searchExpression);
        }
        if (combinedExpr != null && mainParam != null)
        {
            return source.Where(Expression.Lambda<Func<T, bool>>(combinedExpr, new ParameterExpression[1] { mainParam }));
        }
        return source;
    }

    private static void ApplyLikeAsOrGroup<T>(ref Expression? combinedExpr, ref ParameterExpression? mainParam, ref ParameterReplacerVisitor? visitor, SearchExpressionInfo<T> searchExpression)
    {
        if (mainParam == null)
        {
            mainParam = searchExpression.Selector.Parameters[0];
        }
        Expression expression = searchExpression.Selector.Body;
        if (mainParam != searchExpression.Selector.Parameters[0])
        {
            if (visitor == null)
            {
                visitor = new ParameterReplacerVisitor(searchExpression.Selector.Parameters[0], mainParam);
            }
            visitor!.Update(searchExpression.Selector.Parameters[0], mainParam);
            expression = visitor!.Visit(expression);
        }
        MemberExpression arg = StringAsExpression(searchExpression.SearchTerm);
        MethodCallExpression methodCallExpression = Expression.Call(null, _likeMethodInfo, _functions, expression, arg);
        combinedExpr = ((combinedExpr == null) ? ((Expression)methodCallExpression) : ((Expression)Expression.OrElse(combinedExpr, methodCallExpression)));
    }
}
