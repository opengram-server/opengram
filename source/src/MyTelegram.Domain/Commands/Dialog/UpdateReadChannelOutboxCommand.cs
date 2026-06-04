namespace MyTelegram.Domain.Commands.Dialog;

public class UpdateReadChannelOutboxCommand(DialogId aggregateId, RequestInfo requestInfo, int maxId)
    : Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    //public long MessageSenderUserId { get; }
    //public long ChannelId { get; }
    public int MaxId { get; } = maxId;

    /*long messageSenderUserId, long channelId, */
    //MessageSenderUserId = messageSenderUserId;
    //ChannelId = channelId;

    public RequestInfo RequestInfo { get; } = requestInfo;
}