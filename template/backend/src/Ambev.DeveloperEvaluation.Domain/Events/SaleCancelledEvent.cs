namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a Sale is successfuilly cancelled.
/// </summary>
/// <param name="SaleId">The unique identifier of the cancelled sale</param>
/// <param name="SaleNumber">The cancelled sale number</param>
/// <param name="Date">The date when the Sale was cancelled</param>
public record SaleCancelledEvent(Guid SaleId, string SaleNumber, DateTime CancelledAt);