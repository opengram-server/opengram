namespace MyTelegram.Domain.Events.User;

/// <summary>
/// Event fired when user's default history TTL setting is updated
/// This applies to all new chats created by the user
/// </summary>
public class UserDefaultHistoryTTLUpdatedEvent(
    RequestInfo requestInfo,
    long userId,
    int period) 
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public long UserId { get; } = userId;
    
    /// <summary>
    /// Default TTL period in seconds
    /// </summary>
    public int Period { get; } = period;
}
