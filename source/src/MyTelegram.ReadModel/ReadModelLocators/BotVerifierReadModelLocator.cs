namespace MyTelegram.ReadModel.ReadModelLocators;

public class BotVerifierReadModelLocator : IBotVerifierReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        if (domainEvent is IDomainEvent<UserAggregate, UserId, BotVerifierCreatedEvent> createdEvent)
        {
            yield return $"bot_{createdEvent.AggregateEvent.BotUserId}";
        }
        else if (domainEvent is IDomainEvent<UserAggregate, UserId, BotVerifierSettingsUpdatedEvent> updatedEvent)
        {
            yield return $"bot_{updatedEvent.AggregateEvent.BotUserId}";
        }
    }
}
