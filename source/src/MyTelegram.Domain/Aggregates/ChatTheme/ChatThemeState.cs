using MyTelegram.Domain.Events.ChatTheme;

namespace MyTelegram.Domain.Aggregates.ChatTheme;

public class ChatThemeState : AggregateState<ChatThemeAggregate, ChatThemeId, ChatThemeState>,
    IApply<ChatThemeSetEvent>,
    IApply<ChatThemeClearedEvent>
{
    public long OwnerPeerId { get; private set; }
    public PeerType PeerType { get; private set; }
    public long PeerId { get; private set; }
    public string? Emoticon { get; private set; }
    public int? MessageId { get; private set; }
    public int SetDate { get; private set; }

    public void Apply(ChatThemeSetEvent aggregateEvent)
    {
        OwnerPeerId = aggregateEvent.OwnerPeerId;
        PeerType = aggregateEvent.PeerType;
        PeerId = aggregateEvent.PeerId;
        Emoticon = aggregateEvent.Emoticon;
        MessageId = aggregateEvent.MessageId;
        SetDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public void Apply(ChatThemeClearedEvent aggregateEvent)
    {
        Emoticon = null;
        MessageId = null;
    }

    public ChatThemeSnapshot ToSnapshot()
    {
        return new ChatThemeSnapshot(
            OwnerPeerId,
            PeerType,
            PeerId,
            Emoticon,
            MessageId,
            SetDate
        );
    }

    public void LoadFromSnapshot(ChatThemeSnapshot snapshot)
    {
        OwnerPeerId = snapshot.OwnerPeerId;
        PeerType = snapshot.PeerType;
        PeerId = snapshot.PeerId;
        Emoticon = snapshot.Emoticon;
        MessageId = snapshot.MessageId;
        SetDate = snapshot.SetDate;
    }
}
