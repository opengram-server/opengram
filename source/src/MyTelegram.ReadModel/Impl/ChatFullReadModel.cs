using MyTelegram.Domain.Aggregates.Chat;
using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Events.Chat;
using MyTelegram.Domain.Events.GroupCall;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Domain;

namespace MyTelegram.ReadModel.Impl;

public class ChatFullReadModel : IChatFullReadModel,
    IAmReadModelFor<ChatAggregate, ChatId, ChatCreatedEvent>,
    IAmReadModelFor<ChatAggregate, ChatId, ChatAvailableReactionsChangedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallCreatedEvent>,
    IAmReadModelFor<GroupCallAggregate, GroupCallId, GroupCallDiscardedEvent>
{
    public string Id { get; set; } = null!;
    public long ChatId { get; private set; }
    public string? About { get; private set; }
    public int? PinnedMsgId { get; private set; }
    public int? FolderId { get; private set; }
    public int? TtlPeriod { get; private set; }
    public ReactionType ReactionType { get; private set; }
    public bool AllowCustomReaction { get; private set; }
    public List<string>? AvailableReactions { get; private set; }
    public int? RequestsPending { get; private set; }
    public List<long>? RecentRequesters { get; private set; }
    public long? Version { get; set; }
    
    // Group call
    public long? ActiveGroupCallId { get; private set; }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChatAggregate, ChatId, ChatCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        ChatId = domainEvent.AggregateEvent.ChatId;
        // Enable all reactions by default for new chats
        ReactionType = ReactionType.ReactionAll;
        AllowCustomReaction = true;
        
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChatAggregate, ChatId, ChatAvailableReactionsChangedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        ReactionType = domainEvent.AggregateEvent.ReactionType;
        AllowCustomReaction = domainEvent.AggregateEvent.AllowCustomReaction;
        AvailableReactions = domainEvent.AggregateEvent.AvailableReactions;
        
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallCreatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        // Only update if this is for our chat
        if (domainEvent.AggregateEvent.PeerType == PeerType.Chat && domainEvent.AggregateEvent.PeerId == ChatId)
        {
            ActiveGroupCallId = domainEvent.AggregateEvent.CallId;
        }
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallDiscardedEvent> domainEvent, CancellationToken cancellationToken)
    {
        // Only update if this is for our chat and this is our active call
        if (domainEvent.AggregateEvent.PeerType == PeerType.Chat && 
            domainEvent.AggregateEvent.PeerId == ChatId &&
            ActiveGroupCallId == domainEvent.AggregateEvent.CallId)
        {
            ActiveGroupCallId = null;
        }
        return Task.CompletedTask;
    }
}
