using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Command for creating a new sale
/// </summary>
public class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    /// <summary>
    /// Represents de entity unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Human friendly sale number (could be sequential or formatted string).
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// External identifier of the customer.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// External identifier of the branch where the sale happened.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Read-only collection of sale items.
    /// </summary>
    public ICollection<SaleItem> Products { get; set; } = [];

    /// <summary>
    /// The aggregated total amount of the sale (sum of item totals).
    /// </summary>
    public decimal TotalAmount { get; set; } = 0;

    /// <summary>
    /// Status of the sale (Active or Cancelled).
    /// </summary>
    public SaleStatus Status { get; set; } = SaleStatus.Active;
}
