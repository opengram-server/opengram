namespace MyTelegram.Domain.Events.Messaging;

public class ReplyChannelMessageCompletedEvent(
    RequestInfo requestInfo,
    long channelId,
    int replyToMessageId,
    MessageReply reply,
    long? postChannelId,
    int? postMessageId) : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public int ReplyToMessageId { get; } = replyToMessageId;
    public MessageReply Reply { get; } = reply;
    public long? PostChannelId { get; } = postChannelId;
    public int? PostMessageId { get; } = postMessageId;
}