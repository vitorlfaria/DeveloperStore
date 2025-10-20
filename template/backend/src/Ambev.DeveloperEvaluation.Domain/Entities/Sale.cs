using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// The aggregate root representing a sale. It contains SaleItems and encapsulates
/// all business rules related to a sale.
/// </summary>
public sealed class Sale : BaseEntity
{
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

    /// <summary>
    /// Create a new Sale aggregate. Use factory method New(...) to ensure proper initialization.
    /// </summary>
    public Sale(Guid customerId, Guid branchId)
    {
        CustomerId = customerId;
        BranchId = branchId;
        SaleNumber = GenerateSaleNumber();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Performs validation of the sale entity using the SaleValidator rules.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing:
    /// - IsValid: Indicates whether all validation rules passed
    /// - Errors: Collection of validation errors if any rules failed
    /// </returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    /// <summary>
    /// Cancels the entire sale. After cancellation no more changes are allowed.
    /// </summary>
    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled) return;
        Status = SaleStatus.Cancelled;
        TotalAmount = 0;
    }

    /// <summary>
    /// Recalculates the aggregated totals for the sale based on current items.
    /// This method must be called after any mutation that affects item totals.
    /// </summary>
    public void RecalculateTotals()
    {
        decimal total = 0;
        foreach (var item in Products)
        {
            item.ApplyDiscountAndRecalculateTotal();
            total += item.Total;
        }

        TotalAmount = total;
    }

    /// <summary>
    /// Generates a readable sale number.
    /// </summary>
    public static string GenerateSaleNumber()
    {
        var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var shortGuid = Guid.NewGuid().ToString().Split('-')[0];
        return $"S-{ts}-{shortGuid}";
    }
}