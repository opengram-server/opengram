namespace MyTelegram.Domain.Events.Channel;

public class StartSendChannelMessageEvent(
    RequestInfo requestInfo,
    long senderPeerId,
    bool senderIsBot,
    bool post,
    int? views,
    int messageId,
    IReadOnlyList<long> botUidList,
    long? linkedChannelId,
    int date)
    : /*Request*/RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    //long reqMsgId,

    public IReadOnlyList<long> BotUidList { get; } = botUidList;
    public int Date { get; } = date;
    public long? LinkedChannelId { get; } = linkedChannelId;
    public int MessageId { get; } = messageId;

    /// <summary>
    /// Whether this is a channel post
    /// </summary>
    public bool Post { get; } = post;

    public bool SenderIsBot { get; } = senderIsBot;
    public long SenderPeerId { get; } = senderPeerId;
    public int? Views { get; } = views;
}
