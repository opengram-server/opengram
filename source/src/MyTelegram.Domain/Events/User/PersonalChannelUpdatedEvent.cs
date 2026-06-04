namespace MyTelegram.Domain.Events.User;

public class PersonalChannelUpdatedEvent(long userId, long? personalChannelId) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public long? PersonalChannelId { get; } = personalChannelId;
}