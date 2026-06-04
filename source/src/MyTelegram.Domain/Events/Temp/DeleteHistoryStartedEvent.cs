namespace MyTelegram.Domain.Events.Temp;

public class DeleteHistoryStartedEvent
    (RequestInfo requestInfo, IReadOnlyCollection<MessageItemToBeDeleted> messageItems, bool revoke, bool deleteGroupMessagesForEveryone, bool isDeletePhoneCallHistory) :
    RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    //public List<int> MessageIds { get; } = messageIds;
    public IReadOnlyCollection<MessageItemToBeDeleted> MessageItems { get; } = messageItems;
    public bool Revoke { get; } = revoke;
    public bool DeleteGroupMessagesForEveryone { get; } = deleteGroupMessagesForEveryone;
    public bool IsDeletePhoneCallHistory { get; } = isDeletePhoneCallHistory;
}