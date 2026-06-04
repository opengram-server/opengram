namespace MyTelegram.Domain.Events.UserName;

public class SetUserNameSuccessEvent(
    RequestInfo requestInfo,
    long selfUserId,
    string userName,
    PeerType peerType,
    long peerId,
    int date
    )
    : RequestAggregateEvent2<UserNameAggregate, UserNameId>(requestInfo)
{
    public long PeerId { get; } = peerId;
    public int Date { get; } = date;
    public PeerType PeerType { get; } = peerType;
    public long SelfUserId { get; } = selfUserId;
    public string UserName { get; } = userName;
}