namespace MyTelegram.Domain.Events.User;

public class UserColorUpdatedEvent(RequestInfo requestInfo, long userId, PeerColor? color, bool forProfile)
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public long UserId { get; } = userId;
    public PeerColor? Color { get; } = color;
    public bool ForProfile { get; } = forProfile;

    /*int color, long? backgroundEmojiId*/
}