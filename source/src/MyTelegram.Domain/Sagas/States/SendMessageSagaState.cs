namespace MyTelegram.Domain.Sagas.States;

public class SendMessageSagaState : AggregateState<SendMessageSaga, SendMessageSagaId, SendMessageSagaState>,
    //IApply<SendMessageSagaStartedEvent>,
    IApply<ReceiveInboxMessageCompletedSagaEvent>,
    IApply<ReplyChannelMessageCompletedEvent>,
    IApply<ReplyBroadcastChannelCompletedSagaEvent>,
    IApply<PostChannelIdUpdatedSagaEvent>,
    IApply<SendMessageStartedSagaEvent>,
    IApply<OutboxMessageCreatedSagaEvent>,
    IApply<SendOutboxMessageCompletedSagaEvent>,
    IApply<InboxMessageCreatedSagaEvent>
{
    public RequestInfo RequestInfo { get; set; } = null!;
    public List<long>? MentionedUserIds { get; private set; }
    public long? LinkedChannelId { get; set; }
    public List<long>? ChatMembers { get; private set; } = new();
    public Dictionary<Guid, List<InboxItem>> UserInboxItems { get; set; } = new();
    public List<SendMessageItem> SendMessageItems { get; private set; } = [];
    public bool IsSendQuickReplyMessages { get; private set; }
    public bool IsSendGroupedMessages { get; private set; }
    public bool ClearDraft { get; private set; }
    public int SentCount { get; private set; }

    public int InboxReceiveCount { get; private set; }

    public int TotalCount { get; private set; }

    public SendMessageItem FirstMessageItem { get; private set; } = null!;

    public List<MessageItem> InboxMessageItems { get; private set; } = [];

    public bool IsSendOutboxMessageCompleted => SentCount == SendMessageItems.Count;

    public void Apply(SendMessageStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
        SendMessageItems = aggregateEvent.SendMessageItems;
        IsSendGroupedMessages = aggregateEvent.IsSendGroupedMessages;
        IsSendQuickReplyMessages = aggregateEvent.IsSendQuickReplyMessages;
        ClearDraft = aggregateEvent.ClearDraft;
        ChatMembers = aggregateEvent.ChatMembers;

        FirstMessageItem = aggregateEvent.SendMessageItems.First();

        TotalCount = SendMessageItems.Count;
        if (aggregateEvent.ChatMembers?.Count > 0)
        {
            TotalCount = SendMessageItems.Count * aggregateEvent.ChatMembers.Count;
        }
    }

    public void Apply(ReceiveInboxMessageCompletedSagaEvent aggregateEvent)
    {
        //InboxItems.Add(new(aggregateEvent.MessageItem.OwnerPeer.PeerId, aggregateEvent.MessageItem.MessageId));
    }

    public bool IsCreateInboxMessagesCompleted()
    {
        // For User: InboxReceiveCount should equal SendMessageItems.Count (1 inbox per message)
        // For Chat/Channel: InboxReceiveCount should equal SendMessageItems.Count * (ChatMembers.Count - 1)
        // because we skip the sender
        if (ChatMembers == null || ChatMembers.Count == 0)
        {
            return InboxReceiveCount == SendMessageItems.Count;
        }
        
        // Calculate expected inbox count: messages * (members - sender)
        var expectedInboxCount = SendMessageItems.Sum(item => 
        {
            var itemChatMembers = item.ChatMembers ?? [];
            return itemChatMembers.Count(memberId => memberId != RequestInfo.UserId);
        });
        
        return InboxReceiveCount == expectedInboxCount;
    }

    public void Apply(ReplyChannelMessageCompletedEvent aggregateEvent)
    {
    }

    public void Apply(ReplyBroadcastChannelCompletedSagaEvent aggregateEvent)
    {
    }

    public void Apply(PostChannelIdUpdatedSagaEvent aggregateEvent)
    {
    }

    public void Apply(OutboxMessageCreatedSagaEvent aggregateEvent)
    {
        SentCount++;
    }

    public void Apply(SendOutboxMessageCompletedSagaEvent aggregateEvent)
    {

    }

    public void Apply(InboxMessageCreatedSagaEvent aggregateEvent)
    {
        InboxMessageItems.Add(aggregateEvent.MessageItem);
        var key = aggregateEvent.MessageItem.BatchId ?? Guid.Empty;
        //InboxItems.Add(new InboxItem(aggregateEvent.MessageItem.OwnerPeer.PeerId, aggregateEvent.MessageItem.MessageId));
        if (!UserInboxItems.TryGetValue(key, out var inboxItems))
        {
            inboxItems = new List<InboxItem>();
            UserInboxItems.TryAdd(key, inboxItems);
        }

        inboxItems.Add(new InboxItem(aggregateEvent.MessageItem.OwnerPeer.PeerId, aggregateEvent.MessageItem.MessageId));
        InboxReceiveCount++;
    }
}