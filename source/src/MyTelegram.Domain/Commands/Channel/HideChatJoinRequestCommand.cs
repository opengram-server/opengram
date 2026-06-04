namespace MyTelegram.Domain.Commands.Channel;

public class HideChatJoinRequestCommand(
    JoinChannelId aggregateId,
    RequestInfo requestInfo,
    long userId,
    bool approved,
    int topMessageId,
    int channelHistoryMinId,
    bool broadcast) : RequestCommand2<JoinChannelAggregate, JoinChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long UserId { get; } = userId;
    public bool Approved { get; } = approved;
    public int TopMessageId { get; } = topMessageId;
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;
    public bool Broadcast { get; } = broadcast;
}