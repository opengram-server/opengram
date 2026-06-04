namespace MyTelegram.Domain.Commands.Temp;

public class StartDeleteHistoryCommand(
    TempId aggregateId,
    RequestInfo requestInfo,
    //List<int> messageIds,
    IReadOnlyCollection<MessageItemToBeDeleted> messageItems,
    bool revoke,
    bool deleteGroupMessagesForEveryone,
    bool isDeletePhoneCallHistory = false
    )
    : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    //public List<int> MessageIds { get; } = messageIds;
    public IReadOnlyCollection<MessageItemToBeDeleted> MessageItems { get; } = messageItems;
    public bool Revoke { get; } = revoke;
    public bool DeleteGroupMessagesForEveryone { get; } = deleteGroupMessagesForEveryone;
    public bool IsDeletePhoneCallHistory { get; } = isDeletePhoneCallHistory;
}