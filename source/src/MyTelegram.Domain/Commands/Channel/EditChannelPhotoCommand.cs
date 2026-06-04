namespace MyTelegram.Domain.Commands.Channel;

public class EditChannelPhotoCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    long? fileId,
    IMessageAction messageAction,
    long randomId)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long? FileId { get; } = fileId;
    public IMessageAction MessageAction { get; } = messageAction;
    public long RandomId { get; } = randomId;
}
