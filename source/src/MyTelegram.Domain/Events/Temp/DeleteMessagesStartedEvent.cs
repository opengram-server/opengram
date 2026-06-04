namespace MyTelegram.Domain.Events.Temp;

public class DeleteMessagesStartedEvent(RequestInfo requestInfo, IReadOnlyCollection<MessageItemToBeDeleted> messageItems, bool revoke, bool deleteGroupMessagesForEveryone, int? newTopMessageId, int? newTopMessageIdForOtherParticipant) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public IReadOnlyCollection<MessageItemToBeDeleted> MessageItems { get; } = messageItems;
    public bool Revoke { get; } = revoke;
    public bool DeleteGroupMessagesForEveryone { get; } = deleteGroupMessagesForEveryone;
    public int? NewTopMessageId { get; } = newTopMessageId;
    public int? NewTopMessageIdForOtherParticipant { get; } = newTopMessageIdForOtherParticipant;
}