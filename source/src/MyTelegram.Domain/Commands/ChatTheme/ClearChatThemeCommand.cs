using MyTelegram.Domain.Aggregates.ChatTheme;

namespace MyTelegram.Domain.Commands.ChatTheme;

public class ClearChatThemeCommand(
    ChatThemeId aggregateId,
    RequestInfo requestInfo,
    long ownerPeerId,
    PeerType peerType,
    long peerId) : RequestCommand2<ChatThemeAggregate, ChatThemeId, IExecutionResult>(aggregateId, requestInfo)
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public PeerType PeerType { get; } = peerType;
    public long PeerId { get; } = peerId;
}
