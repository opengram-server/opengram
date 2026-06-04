namespace MyTelegram.Domain.Commands.Temp;

public class StartUnpinAllMessagesCommand(TempId aggregateId, RequestInfo requestInfo, IReadOnlyCollection<SimpleMessageItem> messageItems, Peer toPeer) : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public IReadOnlyCollection<SimpleMessageItem> MessageItems { get; } = messageItems;
    public Peer ToPeer { get; } = toPeer;
}