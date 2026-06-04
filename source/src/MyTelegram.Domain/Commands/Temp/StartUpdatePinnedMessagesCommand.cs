namespace MyTelegram.Domain.Commands.Temp;

public class StartUpdatePinnedMessagesCommand(
    TempId aggregateId,
    RequestInfo requestInfo,
    IReadOnlyCollection<SimpleMessageItem> messageItems,
    Peer toPeer,
    bool pinned,
    bool pmOneSide
    ) : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public IReadOnlyCollection<SimpleMessageItem> MessageItems { get; } = messageItems;
    public Peer ToPeer { get; } = toPeer;
    public bool Pinned { get; } = pinned;
    public bool PmOneSide { get; } = pmOneSide;
}