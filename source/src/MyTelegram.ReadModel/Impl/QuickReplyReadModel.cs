using MyTelegram.Domain.Aggregates.QuickReply;
using MyTelegram.Domain.Events.QuickReply;
using MyTelegram.Domain.Shared.QuickReply;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.ReadModel.Impl;

public class QuickReplyReadModel : IQuickReplyReadModel,
    IAmReadModelFor<QuickReplyAggregate, QuickReplyId, QuickReplyShortcutCreatedEvent>,
    IAmReadModelFor<QuickReplyAggregate, QuickReplyId, QuickReplyShortcutUpdatedEvent>,
    IAmReadModelFor<QuickReplyAggregate, QuickReplyId, QuickReplyShortcutDeletedEvent>,
    IAmReadModelFor<QuickReplyAggregate, QuickReplyId, QuickRepliesReorderedEvent>
{
    public string Id { get; set; } = null!;
    public virtual long? Version { get; set; }
    
    public long UserId { get; private set; }
    public List<QuickReplyShortcutItem> Shortcuts { get; private set; } = new();

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<QuickReplyAggregate, QuickReplyId, QuickReplyShortcutCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        UserId = domainEvent.AggregateEvent.OwnerUserId;
        Shortcuts.Add(new QuickReplyShortcutItem
        {
            ShortcutId = domainEvent.AggregateEvent.ShortcutId,
            Shortcut = domainEvent.AggregateEvent.Shortcut
        });
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<QuickReplyAggregate, QuickReplyId, QuickReplyShortcutUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var item = Shortcuts.FirstOrDefault(p => p.ShortcutId == domainEvent.AggregateEvent.ShortcutId);
        if (item != null)
        {
            item.Shortcut = domainEvent.AggregateEvent.NewShortcut;
        }
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<QuickReplyAggregate, QuickReplyId, QuickReplyShortcutDeletedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Shortcuts.RemoveAll(p => p.ShortcutId == domainEvent.AggregateEvent.ShortcutId);
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<QuickReplyAggregate, QuickReplyId, QuickRepliesReorderedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var newShortcuts = new List<QuickReplyShortcutItem>();
        foreach (var id in domainEvent.AggregateEvent.Order)
        {
            var item = Shortcuts.FirstOrDefault(p => p.ShortcutId == id);
            if (item != null)
            {
                newShortcuts.Add(item);
            }
        }
        // Add missing
        foreach (var item in Shortcuts)
        {
            if (!newShortcuts.Contains(item))
            {
                newShortcuts.Add(item);
            }
        }
        Shortcuts = newShortcuts;
        return Task.CompletedTask;
    }
}
