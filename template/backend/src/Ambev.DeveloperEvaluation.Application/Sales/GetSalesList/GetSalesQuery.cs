using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSalesList;

public class GetSalesQuery : IRequest<IEnumerable<Sale>>
{
    /// <summary>
    /// The number os the requested page
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of items to be returned
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// Defines the order to be applied
    /// </summary>
    public string? OrderBy { get; set; } = null;

    /// <summary>
    /// Defines the filters to be applied
    /// </summary>
    public Dictionary<string, string>? Filters { get; set; } = null;
}
