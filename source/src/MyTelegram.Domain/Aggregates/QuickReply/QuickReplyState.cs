using MyTelegram.Domain.Events.QuickReply;
using MyTelegram.Domain.Shared.QuickReply;

namespace MyTelegram.Domain.Aggregates.QuickReply;

public class QuickReplyState : AggregateState<QuickReplyAggregate, QuickReplyId, QuickReplyState>,
    IApply<QuickReplyShortcutCreatedEvent>,
    IApply<QuickReplyShortcutUpdatedEvent>,
    IApply<QuickReplyShortcutDeletedEvent>,
    IApply<QuickRepliesReorderedEvent>
{
    public long OwnerUserId { get; private set; }
    public List<QuickReplyShortcutItem> Shortcuts { get; private set; } = new();

    public void Apply(QuickReplyShortcutCreatedEvent aggregateEvent)
    {
        OwnerUserId = aggregateEvent.OwnerUserId;
        Shortcuts.Add(new QuickReplyShortcutItem
        {
            ShortcutId = aggregateEvent.ShortcutId,
            Shortcut = aggregateEvent.Shortcut
        });
    }

    public void Apply(QuickReplyShortcutUpdatedEvent aggregateEvent)
    {
        var item = Shortcuts.FirstOrDefault(p => p.ShortcutId == aggregateEvent.ShortcutId);
        if (item != null)
        {
            item.Shortcut = aggregateEvent.NewShortcut;
        }
    }

    public void Apply(QuickReplyShortcutDeletedEvent aggregateEvent)
    {
        Shortcuts.RemoveAll(p => p.ShortcutId == aggregateEvent.ShortcutId);
    }

    public void Apply(QuickRepliesReorderedEvent aggregateEvent)
    {
        // Reorder logic: create new list based on order
        var newShortcuts = new List<QuickReplyShortcutItem>();
        foreach (var id in aggregateEvent.Order)
        {
            var item = Shortcuts.FirstOrDefault(p => p.ShortcutId == id);
            if (item != null)
            {
                newShortcuts.Add(item);
            }
        }
        // Add any missing ones (shouldn't happen if validation is correct, but safety first)
        foreach (var item in Shortcuts)
        {
            if (!newShortcuts.Contains(item))
            {
                newShortcuts.Add(item);
            }
        }
        Shortcuts = newShortcuts;
    }
}
