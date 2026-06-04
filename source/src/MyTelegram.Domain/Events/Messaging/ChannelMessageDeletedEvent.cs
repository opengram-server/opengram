namespace MyTelegram.Domain.Events.Messaging;

public class ChannelMessageDeletedEvent(
    RequestInfo requestInfo,
    long channelId,
    int messageId,
    bool isThisMessageForwardFromChannelPost,
    long? postChannelId,
    int? postMessageId)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public int MessageId { get; } = messageId;
    public bool IsThisMessageForwardFromChannelPost { get; } = isThisMessageForwardFromChannelPost;
    public long? PostChannelId { get; } = postChannelId;
    public int? PostMessageId { get; } = postMessageId;
}