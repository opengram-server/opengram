namespace MyTelegram.Messenger.Services.Caching;

public interface ICachedReadModelUpdater
{
    Task UpdateAsync(IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken);
}