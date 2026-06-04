namespace MyTelegram.Domain.Commands.Channel;

public class HideChatJoinRequestCommand2(
    JoinChannelId aggregateId,
    RequestInfo requestInfo,
    long userId,
    bool approved,
    int topMessageId,
    int channelHistoryMinId,
    bool broadcast)
    : Command<JoinChannelAggregate, JoinChannelId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long UserId { get; } = userId;
    public bool Approved { get; } = approved;
    public int TopMessageId { get; } = topMessageId;
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;
    public bool Broadcast { get; } = broadcast;
}