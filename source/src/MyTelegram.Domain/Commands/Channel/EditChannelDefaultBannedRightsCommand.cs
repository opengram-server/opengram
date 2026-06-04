namespace MyTelegram.Domain.Commands.Channel;

public class EditChannelDefaultBannedRightsCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    ChatBannedRights chatBannedRights,
    long selfUserId)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public ChatBannedRights ChatBannedRights { get; } = chatBannedRights;
    public long SelfUserId { get; } = selfUserId;
}
