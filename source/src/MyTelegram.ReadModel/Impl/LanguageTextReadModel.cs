using MyTelegram.Domain.Aggregates.Language;

namespace MyTelegram.ReadModel.Impl;

public class LanguageTextReadModel : ILanguageTextReadModel,
    IAmReadModelFor<LanguageTextAggregate, LanguageTextId, LanguageTextCreatedEvent>
{
    public DeviceType Platform { get; private set; }
    public string LanguageCode { get; private set; } = null!;
    public string Key { get; private set; } = null!;
    public string Value { get; private set; } = null!;
    public int LanguageVersion { get; private set; }

    public virtual string Id { get; private set; } = null!;
    public virtual long? Version { get; set; }
    public Task ApplyAsync(IReadModelContext context, IDomainEvent<LanguageTextAggregate, LanguageTextId, LanguageTextCreatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}