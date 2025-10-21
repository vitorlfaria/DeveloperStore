namespace Ambev.DeveloperEvaluation.Domain.Events;

public interface IHasDomainEvents
{
    IReadOnlyCollection<object> DomainEvents { get; }
    void AddDomainEvent(object @event);
    void ClearDomainEvents();
}
