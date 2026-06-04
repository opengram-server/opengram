namespace MyTelegram.Domain.Commands.Temp;

public class StartDeleteMessagesCommand(
    TempId aggregateId,
    RequestInfo requestInfo,
    //List<int> messageIds,
    IReadOnlyCollection<MessageItemToBeDeleted> messageItems,
    bool revoke,
    bool deleteGroupMessagesForEveryone,
    int? newTopMessageId,
    int? newTopMessageIdForOtherParticipant
)
    : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    //public List<int> MessageIds { get; } = messageIds;
    public IReadOnlyCollection<MessageItemToBeDeleted> MessageItems { get; } = messageItems;
    public bool Revoke { get; } = revoke;
    public bool DeleteGroupMessagesForEveryone { get; } = deleteGroupMessagesForEveryone;
    public int? NewTopMessageId { get; } = newTopMessageId;
    public int? NewTopMessageIdForOtherParticipant { get; } = newTopMessageIdForOtherParticipant;
}