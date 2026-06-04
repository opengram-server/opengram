namespace MyTelegram.Domain.Events.ChatInvite;

public class ChatInviteDeletedEvent(RequestInfo requestInfo, long channelId, long inviteId)
    : RequestAggregateEvent2<ChatInviteAggregate, ChatInviteId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long InviteId { get; } = inviteId;
}