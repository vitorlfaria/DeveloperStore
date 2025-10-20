using System.ComponentModel;
using System.Linq.Expressions;
using Ambev.DeveloperEvaluation.Common.Extensions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSalesList;

public class GetSalesHandler : IRequestHandler<GetSalesQuery, IEnumerable<Sale>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Sale>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var query = _saleRepository.Query(cancellationToken);
        query = ApplyFilters(query, request.Filters);
        query = ApplyOrdering(query, request.OrderBy);

        var items = await query
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .ToListAsync(cancellationToken);

        return items;
    }

    private IQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return query.OrderBy(s => s.CreatedAt);

        var orders = orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries);
        bool first = true;
        foreach (var order in orders)
        {
            var parts = order.Trim().Split(' ');
            var property = parts[0];
            var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            query = first
                ? (descending ? query.OrderByDescendingDynamic(property) : query.OrderByDynamic(property))
                : (descending ? ((IOrderedQueryable<Sale>)query).ThenByDescendingDynamic(property) : ((IOrderedQueryable<Sale>)query).ThenByDynamic(property));

            first = false;
        }

        return query;
    }

    /// <summary>
    /// Applies the filters to the query
    /// </summary>
    /// <param name="query"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    private static IQueryable<Sale> ApplyFilters(IQueryable<Sale> query, Dictionary<string, string>? filters)
    {
        if (filters is null || filters.Count == 0)
            return query;

        var parameter = Expression.Parameter(typeof(Sale), "x");
        Expression? combined = null;

        foreach (var kvp in filters)
        {
            var field = kvp.Key;
            var value = kvp.Value;

            string propertyName = field;
            bool isMin = false;
            bool isMax = false;
            bool isPartial = false;

            if (field.StartsWith("_min", StringComparison.OrdinalIgnoreCase))
            {
                isMin = true;
                propertyName = field[4..];
            }
            else if (field.StartsWith("_max", StringComparison.OrdinalIgnoreCase))
            {
                isMax = true;
                propertyName = field[4..];
            }

            propertyName = char.ToUpper(propertyName[0]) + propertyName[1..];

            var property = typeof(Sale).GetProperty(propertyName);
            if (property == null)
                continue;

            var member = Expression.Property(parameter, property);

            object? typedValue = ConvertValue(property.PropertyType, value);
            if (typedValue == null)
                continue;

            Expression comparison;

            if (isMin)
            {
                comparison = Expression.GreaterThanOrEqual(
                    member,
                    Expression.Constant(typedValue)
                );
            }
            else if (isMax)
            {
                comparison = Expression.LessThanOrEqual(
                    member,
                    Expression.Constant(typedValue)
                );
            }
            else if (property.PropertyType == typeof(string))
            {
                if (value.StartsWith('*') || value.EndsWith('*'))
                {
                    var clean = value.Trim('*');
                    var method = typeof(string).GetMethod("Contains", [typeof(string)])!;
                    comparison = Expression.Call(member, method, Expression.Constant(clean));
                }
                else
                {
                    comparison = Expression.Equal(
                        member,
                        Expression.Constant(typedValue)
                    );
                }
            }
            else
            {
                comparison = Expression.Equal(
                    member,
                    Expression.Constant(typedValue)
                );
            }

            combined = combined == null
                ? comparison
                : Expression.AndAlso(combined, comparison);
        }

        if (combined == null)
            return query;

        var lambda = Expression.Lambda<Func<Sale, bool>>(combined, parameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Converts string values from query parameters into strongly typed values matching the target property.
    /// Supports Guid, bool, int, decimal, DateTime, enum, etc.
    /// </summary>
    private static object? ConvertValue(Type propertyType, string value)
    {
        if (propertyType == typeof(Guid))
            return Guid.TryParse(value, out var guid) ? guid : null;

        if (propertyType == typeof(bool))
            return bool.TryParse(value, out var b) ? b : null;

        if (propertyType == typeof(int))
            return int.TryParse(value, out var i) ? i : null;

        if (propertyType == typeof(decimal))
            return decimal.TryParse(value, out var d) ? d : null;

        if (propertyType == typeof(DateTime))
            return DateTime.TryParse(value, out var dt) ? dt : null;

        if (propertyType.IsEnum)
            return Enum.TryParse(propertyType, value, true, out var e) ? e : null;

        // fallback to string
        return value;
    }
}
