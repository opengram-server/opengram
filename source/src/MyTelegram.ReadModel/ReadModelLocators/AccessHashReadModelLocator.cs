namespace MyTelegram.ReadModel.ReadModelLocators;

public class AccessHashReadModelLocator : IAccessHashReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var aggregateEvent = domainEvent.GetAggregateEvent();
        switch (aggregateEvent)
        {
            case UserCreatedEvent userCreatedEvent:
                yield return AccessHashId.Create(userCreatedEvent.UserId, userCreatedEvent.AccessHash).Value;
                break;
            case ChannelCreatedEvent channelCreatedEvent:
                yield return AccessHashId.Create(channelCreatedEvent.ChannelId, channelCreatedEvent.AccessHash).Value;
                break;
        }
    }
}