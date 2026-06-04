using MyTelegram.Domain.Events.QuickReply;

namespace MyTelegram.Domain.Aggregates.QuickReply;

public class QuickReplyAggregate : AggregateRoot<QuickReplyAggregate, QuickReplyId>
{
    private readonly QuickReplyState _state = new();

    public QuickReplyAggregate(QuickReplyId id) : base(id)
    {
        Register(_state);
    }

    public void CreateShortcut(RequestInfo requestInfo, int shortcutId, string shortcut)
    {
        //Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this); // Aggregate might exist
        if (_state.Shortcuts.Any(p => p.Shortcut.Equals(shortcut, StringComparison.OrdinalIgnoreCase)))
        {
            RpcErrors.RpcErrors400.ShortNameOccupied.ThrowRpcError();
        }

        Emit(new QuickReplyShortcutCreatedEvent(requestInfo, _state.OwnerUserId == 0 ? requestInfo.UserId : _state.OwnerUserId, shortcutId, shortcut));
    }

    public void UpdateShortcut(RequestInfo requestInfo, int shortcutId, string newShortcut)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        if (_state.Shortcuts.Any(p => p.ShortcutId != shortcutId && p.Shortcut.Equals(newShortcut, StringComparison.OrdinalIgnoreCase)))
        {
            RpcErrors.RpcErrors400.ShortNameOccupied.ThrowRpcError();
        }
        
        Emit(new QuickReplyShortcutUpdatedEvent(requestInfo, _state.OwnerUserId, shortcutId, newShortcut));
    }

    public void DeleteShortcut(RequestInfo requestInfo, int shortcutId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new QuickReplyShortcutDeletedEvent(requestInfo, _state.OwnerUserId, shortcutId));
    }

    public void ReorderShortcuts(RequestInfo requestInfo, List<int> order)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new QuickRepliesReorderedEvent(requestInfo, _state.OwnerUserId, order));
    }
    
    // Note: Messages within a shortcut are likely handled by MessageAggregate, 
    // but linked to this shortcut via a special Peer or ID.
    // Telegram uses a special peer type or ID for quick reply messages?
    // Actually, quick reply messages are just messages with a special flag or stored differently.
    // But for the Aggregate, we primarily manage the Shortcut definition itself.
}
