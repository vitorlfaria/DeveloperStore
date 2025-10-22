using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Handles domain events raised by entities.
/// For this challenge, events are logged instead of published to a broker.
/// </summary>
public class DomainEventDispatcher
{
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(ILogger<DomainEventDispatcher> logger)
    {
        _logger = logger;
    }

    public Task DispatchAsync(IEnumerable<object> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            _logger.LogInformation(
                "Domain event published: {EventType} | Payload: {@Event}",
                @event.GetType().Name,
                @event
            );
        }

        return Task.CompletedTask;
    }
}
