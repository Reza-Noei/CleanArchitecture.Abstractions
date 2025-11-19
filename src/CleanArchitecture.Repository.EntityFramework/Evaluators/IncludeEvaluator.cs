using CleanArchitecture.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

public class IncludeEvaluator : IEvaluator
{
    private readonly struct CacheKey : IEquatable<CacheKey>
    {
        public Type EntityType { get; init; }

        public Type PropertyType { get; init; }

        public Type? PreviousReturnType { get; init; }

        public CacheKey(Type EntityType, Type PropertyType, Type? PreviousReturnType)
        {
            this.EntityType = EntityType;
            this.PropertyType = PropertyType;
            this.PreviousReturnType = PreviousReturnType;
        }

        [CompilerGenerated]
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("CacheKey");
            stringBuilder.Append(" { ");
            if (PrintMembers(stringBuilder))
            {
                stringBuilder.Append(' ');
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString();
        }

        [CompilerGenerated]
        private bool PrintMembers(StringBuilder builder)
        {
            builder.Append("EntityType = ");
            builder.Append(EntityType);
            builder.Append(", PropertyType = ");
            builder.Append(PropertyType);
            builder.Append(", PreviousReturnType = ");
            builder.Append(PreviousReturnType);
            return true;
        }

        [CompilerGenerated]
        public static bool operator !=(CacheKey left, CacheKey right)
        {
            return !(left == right);
        }

        [CompilerGenerated]
        public static bool operator ==(CacheKey left, CacheKey right)
        {
            return left.Equals(right);
        }

        [CompilerGenerated]
        public override int GetHashCode()
        {
            return (EqualityComparer<Type>.Default.GetHashCode(EntityType) * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(PropertyType)) * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(PreviousReturnType);
        }

        [CompilerGenerated]
        public override bool Equals(object obj)
        {
            if (obj is CacheKey)
            {
                return Equals((CacheKey)obj);
            }
            return false;
        }

        [CompilerGenerated]
        public bool Equals(CacheKey other)
        {
            if (EqualityComparer<Type>.Default.Equals(EntityType, other.EntityType) && EqualityComparer<Type>.Default.Equals(PropertyType, other.PropertyType))
            {
                return EqualityComparer<Type>.Default.Equals(PreviousReturnType, other.PreviousReturnType);
            }
            return false;
        }

        [CompilerGenerated]
        public void Deconstruct(out Type EntityType, out Type PropertyType, out Type? PreviousReturnType)
        {
            EntityType = this.EntityType;
            PropertyType = this.PropertyType;
            PreviousReturnType = this.PreviousReturnType;
        }
    }

    private static readonly MethodInfo _includeMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethods("Include").Single((MethodInfo mi) => mi.IsPublic && mi.GetGenericArguments().Length == 2 && mi.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) && mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));

    private static readonly MethodInfo _thenIncludeAfterReferenceMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethods("ThenInclude").Single((MethodInfo mi) => mi.IsPublic && mi.GetGenericArguments().Length == 3 && mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter && mi.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IIncludableQueryable<,>) && mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));

    private static readonly MethodInfo _thenIncludeAfterEnumerableMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethods("ThenInclude").Single((MethodInfo mi) => mi.IsPublic && mi.GetGenericArguments().Length == 3 && !mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter && mi.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IIncludableQueryable<,>) && mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));

    private static readonly ConcurrentDictionary<CacheKey, Func<IQueryable, LambdaExpression, IQueryable>> _cache = new ConcurrentDictionary<CacheKey, Func<IQueryable, LambdaExpression, IQueryable>>();

    public static IncludeEvaluator Instance = new IncludeEvaluator();

    public bool IsCriteriaEvaluator => false;

    private IncludeEvaluator()
    {
    }

    /// <inheritdoc />
    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification is Specification<T> val)
        {
            if (val.OneOrManyIncludeExpressions.IsEmpty)
            {
                return query;
            }

            IncludeExpressionInfo singleOrDefault = val.OneOrManyIncludeExpressions.SingleOrDefault;
            if (singleOrDefault != null)
            {
                LambdaExpression lambdaExpression = singleOrDefault.LambdaExpression;
                CacheKey key = new CacheKey(typeof(T), lambdaExpression.ReturnType, null);
                return (IQueryable<T>)_cache.GetOrAdd(key, new Func<CacheKey, Func<IQueryable, LambdaExpression, IQueryable>>(CreateIncludeDelegate))(query, lambdaExpression);
            }
        }
        foreach (IncludeExpressionInfo includeExpression in specification.IncludeExpressions)
        {
            LambdaExpression lambdaExpression2 = includeExpression.LambdaExpression;
            if ((int)includeExpression.Type == 1)
            {
                CacheKey key2 = new CacheKey(typeof(T), lambdaExpression2.ReturnType, null);
                query = (IQueryable<T>)_cache.GetOrAdd(key2, new Func<CacheKey, Func<IQueryable, LambdaExpression, IQueryable>>(CreateIncludeDelegate))(query, lambdaExpression2);
            }
            else if ((int)includeExpression.Type == 2)
            {
                CacheKey key3 = new CacheKey(typeof(T), lambdaExpression2.ReturnType, includeExpression.PreviousPropertyType);
                query = (IQueryable<T>)_cache.GetOrAdd(key3, new Func<CacheKey, Func<IQueryable, LambdaExpression, IQueryable>>(CreateThenIncludeDelegate))(query, lambdaExpression2);
            }
        }
        return query;
    }

    private static Func<IQueryable, LambdaExpression, IQueryable> CreateIncludeDelegate(CacheKey cacheKey)
    {
        MethodInfo method = _includeMethodInfo.MakeGenericMethod(cacheKey.EntityType, cacheKey.PropertyType);
        ParameterExpression parameterExpression = Expression.Parameter(typeof(IQueryable));
        ParameterExpression parameterExpression2 = Expression.Parameter(typeof(LambdaExpression));
        return Expression.Lambda<Func<IQueryable, LambdaExpression, IQueryable>>(Expression.Call(method, Expression.Convert(parameterExpression, typeof(IQueryable<>)!.MakeGenericType(cacheKey.EntityType)), Expression.Convert(parameterExpression2, typeof(Expression<>)!.MakeGenericType(typeof(Func<,>)!.MakeGenericType(cacheKey.EntityType, cacheKey.PropertyType)))), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
    }

    private static Func<IQueryable, LambdaExpression, IQueryable> CreateThenIncludeDelegate(CacheKey cacheKey)
    {
        Type propertyType;
        MethodInfo method = (IsGenericEnumerable(cacheKey.PreviousReturnType, out propertyType) ? _thenIncludeAfterEnumerableMethodInfo : _thenIncludeAfterReferenceMethodInfo).MakeGenericMethod(cacheKey.EntityType, propertyType, cacheKey.PropertyType);
        ParameterExpression parameterExpression = Expression.Parameter(typeof(IQueryable));
        ParameterExpression parameterExpression2 = Expression.Parameter(typeof(LambdaExpression));
        return Expression.Lambda<Func<IQueryable, LambdaExpression, IQueryable>>(Expression.Call(method, Expression.Convert(parameterExpression, typeof(IIncludableQueryable<,>)!.MakeGenericType(cacheKey.EntityType, cacheKey.PreviousReturnType)), Expression.Convert(parameterExpression2, typeof(Expression<>)!.MakeGenericType(typeof(Func<,>)!.MakeGenericType(propertyType, cacheKey.PropertyType)))), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
    }

    private static bool IsGenericEnumerable(Type type, out Type propertyType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            propertyType = type.GenericTypeArguments[0];
            return true;
        }
        propertyType = type;
        return false;
    }
}
