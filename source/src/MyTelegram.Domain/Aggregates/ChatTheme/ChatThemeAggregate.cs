using MyTelegram.Domain.Events.ChatTheme;

namespace MyTelegram.Domain.Aggregates.ChatTheme;

public class ChatThemeAggregate : MyInMemorySnapshotAggregateRoot<ChatThemeAggregate, ChatThemeId, ChatThemeSnapshot>
{
    private readonly ChatThemeState _state = new();

    public ChatThemeAggregate(ChatThemeId id) : base(id, SnapshotEveryFewVersionsStrategy.Default)
    {
        Register(_state);
    }

    public void SetTheme(RequestInfo requestInfo, long ownerPeerId, PeerType peerType, long peerId, string emoticon, int? messageId)
    {
        Emit(new ChatThemeSetEvent(
            requestInfo,
            ownerPeerId,
            peerType,
            peerId,
            emoticon,
            messageId
        ));
    }

    public void ClearTheme(RequestInfo requestInfo, long ownerPeerId, PeerType peerType, long peerId)
    {
        Emit(new ChatThemeClearedEvent(
            requestInfo,
            ownerPeerId,
            peerType,
            peerId
        ));
    }

    protected override Task<ChatThemeSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_state.ToSnapshot());
    }

    protected override Task LoadSnapshotAsync(ChatThemeSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
    {
        _state.LoadFromSnapshot(snapshot);
        return Task.CompletedTask;
    }
}
