namespace MyTelegram.ReadModel.ReadModelLocators;

public class BotVerificationReadModelLocator : IBotVerificationReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        if (domainEvent is IDomainEvent<UserAggregate, UserId, CustomVerificationSetEvent> userSetEvent)
        {
            yield return $"verification_{(int)VerificationTargetType.User}_{userSetEvent.AggregateEvent.TargetUserId}";
        }
        else if (domainEvent is IDomainEvent<UserAggregate, UserId, CustomVerificationRemovedEvent> userRemovedEvent)
        {
            yield return $"verification_{(int)VerificationTargetType.User}_{userRemovedEvent.AggregateEvent.TargetUserId}";
        }
        else if (domainEvent is IDomainEvent<ChannelAggregate, ChannelId, ChannelCustomVerificationSetEvent> channelSetEvent)
        {
            yield return $"verification_{(int)VerificationTargetType.Channel}_{channelSetEvent.AggregateEvent.ChannelId}";
        }
        else if (domainEvent is IDomainEvent<ChannelAggregate, ChannelId, ChannelCustomVerificationRemovedEvent> channelRemovedEvent)
        {
            yield return $"verification_{(int)VerificationTargetType.Channel}_{channelRemovedEvent.AggregateEvent.ChannelId}";
        }
    }
}
