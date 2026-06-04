namespace MyTelegram.Domain.Sagas.Events;

public class ReplyToChannelMessageCompletedSagaEvent2(
    IInputReplyTo replyTo,
    long channelId,
    int repliesPts,
    int maxId,
    long savedFromPeerId,
    int savedFromMsgId,
    IReadOnlyCollection<Peer> recentRepliers)
    : AggregateEvent<SendMessageSaga, SendMessageSagaId>
{
    public IInputReplyTo ReplyTo { get; } = replyTo;
    public long ChannelId { get; } = channelId;
    public int RepliesPts { get; } = repliesPts;
    public int MaxId { get; } = maxId;
    public long SavedFromPeerId { get; } = savedFromPeerId;
    public int SavedFromMsgId { get; } = savedFromMsgId;
    public IReadOnlyCollection<Peer> RecentRepliers { get; } = recentRepliers;
}