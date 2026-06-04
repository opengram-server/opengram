namespace MyTelegram.Services.Services;

public interface ICachedReadModelManager
{
    Task ApplyUpdatesAsync(IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken);
}