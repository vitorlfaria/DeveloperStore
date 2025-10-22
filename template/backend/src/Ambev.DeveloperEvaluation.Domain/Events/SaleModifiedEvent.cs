namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a Sale is successfuilly modified.
/// </summary>
/// <param name="SaleId">The unique identifier of the modified sale</param>
/// <param name="SaleNumber">The modified sale number</param>
/// <param name="Date">The date when the Sale was modified</param>
public record SaleModifiedEvent(Guid SaleId, string SaleNumber, DateTime ModifiedAt);