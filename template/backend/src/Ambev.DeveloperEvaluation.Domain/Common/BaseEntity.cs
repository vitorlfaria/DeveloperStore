using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Common;

public class BaseEntity
{
    /// <summary>
    /// Represents de entity unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the flag that inform if the entity is deleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time when the entity was updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets the date and time when the entity was deleted.
    /// </summary>
    public DateTime DeletedAt { get; set; }

    /// <summary>
    /// Marks the current entity as deleted and sets the current deleted date.
    /// </summary>
    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
