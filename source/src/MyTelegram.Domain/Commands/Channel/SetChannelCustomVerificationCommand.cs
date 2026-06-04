namespace MyTelegram.Domain.Commands.Channel;

public class SetChannelCustomVerificationCommand : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>
{
    public SetChannelCustomVerificationCommand(
        ChannelId aggregateId,
        RequestInfo requestInfo,
        bool enabled,
        long? botUserId,
        long channelId,
        long iconEmojiId,
        string description,
        string? customDescription) : base(aggregateId, requestInfo)
    {
        Enabled = enabled;
        BotUserId = botUserId;
        ChannelId = channelId;
        IconEmojiId = iconEmojiId;
        Description = description;
        CustomDescription = customDescription;
    }

    public bool Enabled { get; }
    public long? BotUserId { get; }
    public long ChannelId { get; }
    public long IconEmojiId { get; }
    public string Description { get; }
    public string? CustomDescription { get; }
}
