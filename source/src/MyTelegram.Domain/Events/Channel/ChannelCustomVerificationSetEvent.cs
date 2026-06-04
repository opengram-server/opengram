namespace MyTelegram.Domain.Events.Channel;

public class ChannelCustomVerificationSetEvent : RequestAggregateEvent2<ChannelAggregate, ChannelId>
{
    public ChannelCustomVerificationSetEvent(
        RequestInfo requestInfo,
        long channelId,
        long botVerifierId,
        long iconEmojiId,
        string description,
        string? customDescription) : base(requestInfo)
    {
        ChannelId = channelId;
        BotVerifierId = botVerifierId;
        IconEmojiId = iconEmojiId;
        Description = description;
        CustomDescription = customDescription;
    }

    public long ChannelId { get; }
    public long BotVerifierId { get; }
    public long IconEmojiId { get; }
    public string Description { get; }
    public string? CustomDescription { get; }
}
