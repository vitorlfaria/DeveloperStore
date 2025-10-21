using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Command for creating a new sale
/// </summary>
public class CreateSaleCommand : IRequest<CreateSaleResult>
{
    /// <summary>
    /// External identifier of the customer.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// External identifier of the branch where the sale happened.
    /// </summary>
    public Guid BranchId { get; private set; }

    /// <summary>
    /// Read-only collection of sale items.
    /// </summary>
    public ICollection<SaleItem> Products { get; private set; } = [];

    /// <summary>
    /// The aggregated total amount of the sale (sum of item totals).
    /// </summary>
    public decimal TotalAmount { get; private set; } = 0;

    /// <summary>
    /// Status of the sale (Active or Cancelled).
    /// </summary>
    public SaleStatus Status { get; private set; } = SaleStatus.Active;
}
