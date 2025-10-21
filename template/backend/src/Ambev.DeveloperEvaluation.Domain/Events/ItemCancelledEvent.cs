namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when an item of a Sale is successfuilly cancelled.
/// </summary>
/// <param name="SaleId">The unique identifier of the cancelled sale</param>
/// <param name="ProductId">The unique identifier of the cancelled item</param>
/// <param name="Date">The date when the Sale was cancelled</param>
public record ItemCancelledEvent(Guid SaleId, Guid ProductId, DateTime CancelledAt);