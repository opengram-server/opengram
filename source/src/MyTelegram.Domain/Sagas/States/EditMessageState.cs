namespace MyTelegram.Domain.Sagas.States;

public class EditMessageState : AggregateState<EditMessageSaga, EditMessageSagaId, EditMessageState>,
        IApply<EditOutboxMessageStartedSagaEvent>,
        IApply<OutboxMessageEditCompletedSagaEvent>,
        IApply<InboxMessageEditCompletedSagaEvent>,
        IApply<EditInboxMessageStartedSagaEvent>
{
    public Dictionary<long, int> UidToMessageId = new();

    public int CurrentInboxCount { get; private set; }
    public int EditDate { get; private set; }
    public int InboxCount { get; private set; }

    public bool IsCompleted => InboxCount == CurrentInboxCount;

    public RequestInfo RequestInfo { get; private set; } = default!;
    public int SenderMessageId { get; private set; }
    public MessageItem OldMessageItem { get; private set; } = default!;
    public MessageItem NewMessageItem { get; private set; } = default!;

    public MessageItem OldInboxMessageItem { get; private set; } = default!;
    public MessageItem NewInboxMessageItem { get; private set; } = default!;

    public void Apply(EditInboxMessageStartedSagaEvent aggregateEvent)
    {
        //UidToMessageId.TryAdd(aggregateEvent.UserId, aggregateEvent.MessageId);
        CurrentInboxCount++;

        OldInboxMessageItem= aggregateEvent.OldMessageItem;
        NewInboxMessageItem = aggregateEvent.NewMessageItem;
    }

    public void Apply(EditOutboxMessageStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
        OldMessageItem= aggregateEvent.OldMessageItem;
        NewMessageItem = aggregateEvent.NewMessageItem;

        InboxCount = aggregateEvent.NewMessageItem.InboxItems?.Count ?? 0;
    }

    public void Apply(InboxMessageEditCompletedSagaEvent aggregateEvent)
    {
    }

    public void Apply(OutboxMessageEditCompletedSagaEvent aggregateEvent)
    {
    }
}
