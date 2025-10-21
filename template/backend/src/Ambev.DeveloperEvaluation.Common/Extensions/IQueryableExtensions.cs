using System.Linq.Expressions;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.Common.Extensions;

/// <summary>
/// Provides extension methods to perform dynamic ordering on IQueryable sources
/// using property names specified as strings (supports nested properties).
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Orders the sequence by the specified property name (ascending).
    /// </summary>
    /// <typeparam name="T">Element type of the sequence.</typeparam>
    /// <param name="source">Source query.</param>
    /// <param name="propertyName">Property name to order by (supports nested: \"Customer.Name\").</param>
    /// <returns>IQueryable ordered by the property.</returns>
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName)
    {
        return ApplyOrder<T>(source, propertyName, "OrderBy");
    }

    /// <summary>
    /// Orders the sequence by the specified property name (descending).
    /// </summary>
    public static IQueryable<T> OrderByDescendingDynamic<T>(this IQueryable<T> source, string propertyName)
    {
        return ApplyOrder<T>(source, propertyName, "OrderByDescending");
    }

    /// <summary>
    /// Applies a ThenBy on an already ordered query (ascending).
    /// </summary>
    public static IOrderedQueryable<T> ThenByDynamic<T>(this IOrderedQueryable<T> source, string propertyName)
    {
        return (IOrderedQueryable<T>)ApplyOrder<T>(source, propertyName, "ThenBy");
    }

    /// <summary>
    /// Applies a ThenByDescending on an already ordered query (descending).
    /// </summary>
    public static IOrderedQueryable<T> ThenByDescendingDynamic<T>(this IOrderedQueryable<T> source, string propertyName)
    {
        return (IOrderedQueryable<T>)ApplyOrder<T>(source, propertyName, "ThenByDescending");
    }

    /// <summary>
    /// Builds and invokes the OrderBy / OrderByDescending / ThenBy / ThenByDescending method dynamically.
    /// </summary>
    private static IQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, string methodName)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));

        // support nested properties like "Customer.Name"
        var props = propertyName.Split('.', StringSplitOptions.RemoveEmptyEntries);

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression propertyAccess = parameter;
        Type currentType = typeof(T);

        foreach (var prop in props)
        {
            // try to find property ignoring case
            var propertyInfo = currentType.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{prop}' not found on type '{currentType.FullName}'.");
            }

            propertyAccess = Expression.Property(propertyAccess, propertyInfo);
            currentType = propertyInfo.PropertyType;
        }

        // when value type, we want to convert to object? No — keep actual type for generic method
        var lambdaType = typeof(Func<,>).MakeGenericType(typeof(T), currentType);
        var lambda = Expression.Lambda(lambdaType, propertyAccess, parameter);

        // get the generic Queryable.OrderBy/ThenBy method
        var queryableType = typeof(Queryable);
        var method = queryableType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
            .Single();

        var genericMethod = method.MakeGenericMethod(typeof(T), currentType);

        var result = genericMethod.Invoke(null, new object[] { source, lambda });
        return (IQueryable<T>)result!;
    }
}
