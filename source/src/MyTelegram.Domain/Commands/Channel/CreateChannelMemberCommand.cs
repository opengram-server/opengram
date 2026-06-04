namespace MyTelegram.Domain.Commands.Channel;

public class CreateChannelMemberCommand(
    ChannelMemberId aggregateId,
    RequestInfo requestInfo,
    long channelId,
    long userId,
    long inviterId,
    int date,
    bool isBot,
    long? chatInviteId,
    bool isBroadcast,
    ChatJoinType chatJoinType = ChatJoinType.InvitedByAdmin
    )
    : /*Request*/RequestCommand2<ChannelMemberAggregate, ChannelMemberId, IExecutionResult>(aggregateId, requestInfo)
{
    //long reqMsgId,

    public long ChannelId { get; } = channelId;
    public int Date { get; } = date;
    public long InviterId { get; } = inviterId;
    public bool IsBot { get; } = isBot;
    public long? ChatInviteId { get; } = chatInviteId;
    public bool IsBroadcast { get; } = isBroadcast;
    public ChatJoinType ChatJoinType { get; } = chatJoinType;
    public long UserId { get; } = userId;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
        yield return BitConverter.GetBytes(ChannelId);
        yield return BitConverter.GetBytes(UserId);
    }
}