using System.Runtime.CompilerServices;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// Represents a single line item inside a sale.
/// This class is a part of the Sale aggregate and carries its own invariants.
/// </summary>
public sealed class SaleItem
{
    private const decimal TWENTY_PERCENT = 20m;
    private const decimal TEN_PERCENT = 10m;

    /// <summary>
    /// Unique identifier of the sale that this item belongs to.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Unique identifier of the product in the external product context.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Quantity of this product sold.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Unit price of the product at the time of sale.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Discount percentage applied to this item (0 - 100).
    /// </summary>
    public decimal DiscountPercentage { get; private set; }

    /// <summary>
    /// Total monetary value for this item after discount.
    /// </summary>
    public decimal Total { get; private set; }

    /// <summary>
    /// Property that marks a sale item as canceled.
    /// </summary>
    public bool IsCanceled { get; private set; }

    /// <summary>
    /// Creates a new SaleItem instance and calculates totals using business rules.
    /// The constructor validates invariants such as quantity limits.
    /// </summary>
    /// <param name="productId">External product id.</param>
    /// <param name="productName">Product name snapshot.</param>
    /// <param name="unitPrice">Unit price.</param>
    /// <param name="quantity">Quantity to add.</param>
    public SaleItem(Guid productId, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        IsCanceled = false;

        ApplyDiscountAndRecalculateTotal();
    }

    /// <summary>
    /// Applies discount rules based on quantity and calculates the total value for the item.
    /// </summary>
    public void ApplyDiscountAndRecalculateTotal()
    {
        DiscountPercentage = CalculateDiscountPercentage(Quantity);
        var subtotal = UnitPrice * Quantity;
        Total = DiscountPercentage > 0 ? subtotal * (1 - (DiscountPercentage / 100)) : subtotal;
    }

    /// <summary>
    /// Business rule that returns discount percentage for a given quantity.
    /// - quantity &lt; 4 => 0%
    /// - 4..9 => 10%
    /// - 10..20 => 20%
    /// </summary>
    /// <param name="quantity">Quantity to evaluate.</param>
    /// <returns>Discount percentage (0..100).</returns>
    public static decimal CalculateDiscountPercentage(int quantity)
    {
        if (quantity < 4) return 0m;
        if (quantity >= 4 && quantity < 10) return TEN_PERCENT;
        if (quantity >= 10 && quantity <= 20) return TWENTY_PERCENT;
        return 0m;
    }

    /// <summary>
    /// Marks a sale item as cancelled.
    /// </summary>
    public void MarkAsCancelled()
    {
        IsCanceled = true;
    }
}
