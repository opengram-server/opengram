namespace MyTelegram.Domain.Commands.Channel;

public class EditBannedCommand(
    ChannelMemberId aggregateId,
    RequestInfo requestInfo,
    long adminId,
    long channelId,
    long memberUserId,
    ChatBannedRights chatBannedRights)
    : RequestCommand2<ChannelMemberAggregate, ChannelMemberId, IExecutionResult>(aggregateId, requestInfo)
{
    public long AdminId { get; } = adminId;
    public long ChannelId { get; } = channelId;
    public ChatBannedRights ChatBannedRights { get; } = chatBannedRights;
    public long MemberUserId { get; } = memberUserId;
}