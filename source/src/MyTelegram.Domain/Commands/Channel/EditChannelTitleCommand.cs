namespace MyTelegram.Domain.Commands.Channel;

public class EditChannelTitleCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    string title,
    IMessageAction messageAction,
    long randomId)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long RandomId { get; } = randomId;
    public string Title { get; } = title;
    public IMessageAction MessageAction { get; } = messageAction;
}