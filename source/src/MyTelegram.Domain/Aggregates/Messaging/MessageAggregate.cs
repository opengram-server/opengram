namespace MyTelegram.Domain.Aggregates.Messaging;

public class MessageAggregate : SnapshotAggregateRoot<MessageAggregate, MessageId, MessageSnapshot>
{
    private readonly MessageState _state = new();

    public MessageAggregate(MessageId id) : base(id, SnapshotEveryFewVersionsStrategy.Default)
    {
        Register(_state);
    }

    public void UpdateMessagePinned(RequestInfo requestInfo, bool pinned)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new MessagePinnedUpdatedEvent(requestInfo, _state.MessageItem.OwnerPeer.PeerId, _state.MessageItem.MessageId, pinned, _state.MessageItem.ToPeer, _state.MessageItem.Post));
    }

    public void UnpinMessage(RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new MessageUnpinnedEvent(requestInfo, _state.MessageItem.OwnerPeer.PeerId, _state.MessageItem.MessageId));
    }

    public void AddInboxItemsToOutboxMessage(List<InboxItem> inboxItems)
    {
        // Возможна гонка: входящее сообщение может появиться раньше, чем полностью зафиксирован агрегат исходящего.
        // В этом случае не бросаем исключение, а просто выходим — операция будет повторена или обработана позже.
        if (IsNew)
        {
            // Aggregate not created yet - this can happen due to saga timing
            // The inbox items will be added via InboxMessageIdAddedToOutboxMessageEvent later
            return;
        }
        Emit(new InboxItemsAddedToOutboxMessageEvent(inboxItems));
    }

    /// <summary>
    ///     Sender's message id and receiver's message id are independent,add receiver's message id to sender,delete messages
    ///     and pin messages need this
    /// </summary>
    /// <param name="inboxOwnerPeerId"></param>
    /// <param name="inboxMessageId"></param>
    public void AddInboxMessageIdToOutboxMessage(long inboxOwnerPeerId,
        int inboxMessageId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new InboxMessageIdAddedToOutboxMessageEvent(new InboxItem(inboxOwnerPeerId, inboxMessageId)));
    }

    public void CreateInboxMessage(
        RequestInfo requestInfo,
        MessageItem inboxMessageItem,
        int senderMessageId,
        int? senderDefaultHistoryTTL = null)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new InboxMessageCreatedEvent(requestInfo, inboxMessageItem, senderMessageId, senderDefaultHistoryTTL));
    }

    public void CreateOutboxMessage(RequestInfo requestInfo,
        MessageItem outboxMessageItem,
        List<long>? mentionedUserIds,
        List<ReplyToMsgItem>? replyToMsgItems,
        bool clearDraft,
        int groupItemCount,
        long? linkedChannelId,
        List<long>? chatMembers)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
        if (outboxMessageItem.Post)
        {
            var reply = new MessageReply(linkedChannelId, 0, 0, null, null);
            outboxMessageItem = outboxMessageItem with { Views = 1, Reply = reply };
        }
        if (!outboxMessageItem.BatchId.HasValue)
        {
            outboxMessageItem = outboxMessageItem with { BatchId = SequentialGuid.Create() };
        }

        Emit(new OutboxMessageCreatedEvent(requestInfo,
            outboxMessageItem,
            mentionedUserIds,
            replyToMsgItems,
            clearDraft,
            groupItemCount,
            linkedChannelId,
            chatMembers
        ));
    }

    public void DeleteChannelMessage(RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new ChannelMessageDeletedEvent(requestInfo,
            _state.MessageItem.OwnerPeer.PeerId,
            _state.MessageItem.MessageId,
            _state.MessageItem.IsForwardFromChannelPost,
            _state.MessageItem.FwdHeader?.SavedFromPeer?.PeerId,
            _state.MessageItem.FwdHeader?.SavedFromMsgId));
    }

    public void DeleteInboxMessage(RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new InboxMessageDeletedEvent(
            requestInfo,
            _state.MessageItem.OwnerPeer.PeerId,
            _state.MessageItem.MessageId,
            _state.SenderMessageId
        ));
    }

    public void DeleteMessage(RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new MessageDeleted4Event(
            requestInfo,
            _state.MessageItem.ToPeer,
            _state.MessageItem.OwnerPeer.PeerId,
            _state.MessageItem.MessageId,
            _state.MessageItem.IsOut,
            _state.MessageItem.SenderPeer.PeerId,
            _state.SenderMessageId,
            _state.InboxItems));
    }

    public void DeleteOtherPartyMessage(RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new OtherPartyMessageDeletedEvent(
            requestInfo,
            _state.MessageItem.OwnerPeer.PeerId,
            _state.MessageItem.MessageId));
    }

    public void DeleteOutboxMessage(RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new OutboxMessageDeletedEvent(
            requestInfo,
            _state.MessageItem.OwnerPeer.PeerId,
            _state.MessageItem.MessageId,
            _state.InboxItems));
    }

    public void DeleteSelfMessage(RequestInfo requestInfo, int messageId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new SelfMessageDeletedEvent(
            requestInfo,
            _state.MessageItem.OwnerPeer.PeerId,
            messageId,
            _state.MessageItem.IsOut,
            _state.MessageItem.SenderPeer.PeerId,
            _state.SenderMessageId,
            _state.InboxItems
        ));
    }

    public void EditInboxMessage(
        RequestInfo requestInfo,
        int messageId,
        string newMessage,
        int editDate,
        TVector<IMessageEntity>? entities,
        IMessageMedia? media,
        IReplyMarkup? replyMarkup,
        bool invertMedia,
        List<string>? hashtags)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        if (string.IsNullOrEmpty(newMessage))
        {
            newMessage = _state.MessageItem.Message;
        }

        var oldMessageItem = _state.MessageItem;
        media ??= oldMessageItem.Media;

        var newMessageItem = oldMessageItem with
        {
            Message = newMessage,
            Entities = entities,
            Media = media,
            ReplyMarkup = replyMarkup,
            EditDate = editDate,
            InvertMedia = invertMedia,
            Hashtags = hashtags
        };

        Emit(new InboxMessageEditedEventV2(
            requestInfo,
            oldMessageItem,
            newMessageItem));
    }

    public void EditOutboxMessage(RequestInfo requestInfo,
        int messageId,
        string newMessage,
        int editDate,
        TVector<IMessageEntity>? entities,
        IMessageMedia? media,
        IReplyMarkup? replyMarkup,
        bool invertMedia,
        List<string>? hashtags)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        if (_state.MessageItem.Date + MyTelegramConsts.EditTimeLimit < DateTime.UtcNow.ToTimestamp())
        {
            RpcErrors.RpcErrors400.MessageEditTimeExpired.ThrowRpcError();
        }

        if (!_state.MessageItem.IsOut)
        {
            RpcErrors.RpcErrors403.MessageAuthorRequired.ThrowRpcError();
        }

        if (string.IsNullOrEmpty(newMessage))
        {
            newMessage = _state.MessageItem.Message;
        }

        var oldMessageItem = _state.MessageItem;
        media ??= oldMessageItem.Media;

        var newMessageItem = _state.MessageItem with
        {
            Message = newMessage,
            Entities = entities,
            Media = media,
            ReplyMarkup = replyMarkup,
            EditDate = editDate,
            InvertMedia = invertMedia,
            Hashtags = hashtags
        };

        Emit(new OutboxMessageEditedEventV2(requestInfo,
            oldMessageItem,
            newMessageItem
        ));
    }

    public void ForwardMessage(
        RequestInfo requestInfo,
        long randomId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new MessageForwardedEvent(requestInfo, randomId, _state.MessageItem));
    }

    public void IncrementViews()
    {
        //Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        if (!IsNew)
        {
            Emit(new MessageViewsIncrementedEvent(_state.MessageItem.MessageId, _state.MessageItem.Views ?? 0 + 1));
        }
    }

    public void PinChannelMessage(RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new ChannelMessagePinnedEvent(requestInfo, _state.MessageItem.ToPeer.PeerId,
            _state.MessageItem.MessageId));
    }
    public void ReadInboxHistory(RequestInfo requestInfo,
        long readerUid)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new InboxMessageHasReadEvent(requestInfo,
            readerUid,
            _state.MessageItem.MessageId,
            _state.MessageItem.SenderPeer.PeerId,
            _state.SenderMessageId,
            _state.MessageItem.ToPeer,
            _state.MessageItem.SenderPeer.PeerId == readerUid
        ));
    }

    public void ReplyToMessage(RequestInfo requestInfo, Peer replierPeer, int repliesPts, int messageId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        var reply = _state.MessageItem.Reply ?? new MessageReply(null, 0, repliesPts, messageId, new List<Peer>());
        reply.Replies++;
        var recentRepliers = reply.RecentRepliers ?? new List<Peer>();
        var peer = recentRepliers.FirstOrDefault(p => p.PeerId == replierPeer.PeerId);
        if (peer != null)
        {
            recentRepliers.Remove(peer);
        }

        if (recentRepliers.Count > MyTelegramConsts.MaxRecentRepliersCount)
        {
            recentRepliers.RemoveAt(MyTelegramConsts.MaxRecentRepliersCount - 1);
        }

        recentRepliers.Insert(0, replierPeer);

        long? postChannelId = null;
        int? postMessageId = null;
        if (_state.MessageItem.FwdHeader?.ForwardFromLinkedChannel ?? false)
        {
            postChannelId = _state.MessageItem.PostChannelId;
            postMessageId = _state.MessageItem.PostMessageId;
        }

        reply.RecentRepliers = recentRepliers;
        Emit(new ReplyChannelMessageCompletedEvent(requestInfo, _state.MessageItem.ToPeer.PeerId,
            _state.MessageItem.MessageId, reply, postChannelId, postMessageId));
    }

    public void UpdateInboxMessagePinned(
        RequestInfo requestInfo,
        bool pinned,
        bool pmOneSide,
        bool silent,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        var item = _state.MessageItem;
        Emit(new InboxMessagePinnedUpdatedEvent(
            requestInfo,
            item.OwnerPeer.PeerId,
            item.MessageId,
            pinned,
            pmOneSide,
            silent,
            date,
            item.ToPeer,
            _state.Pts));
    }

    public void UpdateMessageRely(int pts)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new MessageReplyUpdatedEvent(_state.MessageItem.OwnerPeer.PeerId,
            MyTelegramConsts.DeletedChannelIdForChannelPost, _state.MessageItem.MessageId, pts));
    }

    public void UpdateOutboxMessagePinned(
        RequestInfo requestInfo,
        bool pinned,
        bool pmOneSide,
        bool silent,
        int date)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        var item = _state.MessageItem;
        Emit(new OutboxMessagePinnedUpdatedEvent(
            requestInfo,
            item.OwnerPeer.PeerId,
            item.MessageId,
            pinned,
            pmOneSide,
            silent,
            date,
            _state.InboxItems,
            item.SenderPeer.PeerId,
            _state.SenderMessageId,
            item.ToPeer,
            _state.Pts,
            _state.MessageItem.Post
        ));
    }

    public void ConvertScheduledMessageToRegular(RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        // Emit event to convert scheduled message to regular message
        Emit(new ScheduledMessageConvertedToRegularEvent(
            requestInfo,
            _state.MessageItem.OwnerPeer.PeerId,
            _state.MessageItem.MessageId,
            _state.MessageItem.ToPeer,
            _state.MessageItem));
    }

    public void AddReaction(
        RequestInfo requestInfo,
        long senderUserId,
        Peer senderPeer,
        IReaction reaction,
        bool big,
        bool addToRecent,
        int date,
        int count = 1)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        Emit(new MessageReactionAddedEvent(
            requestInfo,
            _state.MessageItem.OwnerPeer.PeerId,
            _state.MessageItem.MessageId,
            senderUserId,
            senderPeer,
            reaction,
            big,
            addToRecent,
            date,
            count));
    }

    public void RemoveReaction(
        RequestInfo requestInfo,
        long senderUserId,
        IReaction reaction)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        Emit(new MessageReactionRemovedEvent(
            requestInfo,
            _state.MessageItem.OwnerPeer.PeerId,
            _state.MessageItem.MessageId,
            senderUserId,
            reaction));
    }

    public void UpdateReactions(
        List<ReactionCount>? reactions,
        List<MessagePeerReaction>? recentReactions)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        Emit(new MessageReactionsUpdatedEvent(
            _state.MessageItem.OwnerPeer.PeerId,
            _state.MessageItem.MessageId,
            reactions,
            recentReactions));
    }

    protected override Task<MessageSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new MessageSnapshot(_state.MessageItem,
            _state.InboxItems,
            _state.SenderMessageId,
            _state.Pinned,
            _state.EditDate,
            //_state.EditHide,
            _state.Edited,
            _state.Pts,
            _state.IsDeleted
        ));
    }

    protected override Task LoadSnapshotAsync(MessageSnapshot snapshot,
      ISnapshotMetadata metadata,
      CancellationToken cancellationToken)
    {
        _state.LoadSnapshot(snapshot);
        return Task.CompletedTask;
    }
}