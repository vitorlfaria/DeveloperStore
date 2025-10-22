namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a Sale is successfuilly created.
/// </summary>
/// <param name="SaleId">The unique identifier of the created sale</param>
/// <param name="SaleNumber">The created sale number</param>
/// <param name="Date">The Sale creation date</param>
public record SaleCreatedEvent(Guid SaleId, string SaleNumber, DateTime CreatedAt);