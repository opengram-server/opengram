using MyTelegram.Domain.Aggregates.ChatTheme;

namespace MyTelegram.Domain.Events.ChatTheme;

public class ChatThemeSetEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    PeerType peerType,
    long peerId,
    string emoticon,
    int? messageId) : RequestAggregateEvent2<ChatThemeAggregate, ChatThemeId>(requestInfo)
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public PeerType PeerType { get; } = peerType;
    public long PeerId { get; } = peerId;
    public string Emoticon { get; } = emoticon;
    public int? MessageId { get; } = messageId;
}
