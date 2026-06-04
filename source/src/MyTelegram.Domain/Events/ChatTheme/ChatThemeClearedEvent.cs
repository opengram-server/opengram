using MyTelegram.Domain.Aggregates.ChatTheme;

namespace MyTelegram.Domain.Events.ChatTheme;

public class ChatThemeClearedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    PeerType peerType,
    long peerId) : RequestAggregateEvent2<ChatThemeAggregate, ChatThemeId>(requestInfo)
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public PeerType PeerType { get; } = peerType;
    public long PeerId { get; } = peerId;
}
