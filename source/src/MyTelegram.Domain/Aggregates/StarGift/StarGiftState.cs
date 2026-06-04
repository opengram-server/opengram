using MyTelegram.Domain.Events.StarGift;

namespace MyTelegram.Domain.Aggregates.StarGift;

public class StarGiftState : AggregateState<StarGiftAggregate, StarGiftId, StarGiftState>,
    IApply<StarGiftInitiatedEvent>,
    IApply<StarGiftSentEvent>,
    IApply<StarGiftSavedEvent>,
    IApply<StarGiftConvertedEvent>,
    IApply<StarGiftUpgradedEvent>,
    IApply<StarGiftTransferredEvent>,
    IApply<StarGiftPinnedToggledEvent>,
    IApply<StarGiftOutboxMessageCreatedEvent>,
    IApply<StarGiftInboxMessageCreatedEvent>,
    IApply<StarGiftListedForResaleEvent>,
    IApply<StarGiftRemovedFromResaleEvent>,
    IApply<StarGiftSoldViaResaleEvent>
{
    public long GiftId { get; private set; }
    public long FromUserId { get; private set; }
    public long ToUserId { get; private set; }
    public long? ToPeerId { get; private set; } // For channel gifts
    public int MessageId { get; private set; }
    public long Stars { get; private set; }
    public long ConvertStars { get; private set; }
    public string? Message { get; private set; }
    public bool NameHidden { get; private set; }
    public bool Saved { get; private set; }
    public bool Converted { get; private set; }
    public bool Upgraded { get; private set; }
    public bool Refunded { get; private set; }
    public bool CanUpgrade { get; private set; }
    public bool Pinned { get; private set; }
    public long? SavedId { get; private set; }
    public long? UpgradeStars { get; private set; }
    public int? UpgradeMsgId { get; private set; }
    public int Date { get; private set; }
    public int? ConvertDate { get; private set; }
    public int? UpgradeDate { get; private set; }
    public byte[]? GiftSticker { get; private set; } // Serialized sticker document
    public bool IsDeleted { get; private set; }
    
    // Resale fields
    public bool ForResale { get; private set; }
    public long? ResaleStars { get; private set; }
    public int? ResaleDate { get; private set; }
    public int? SoldDate { get; private set; }
    public long? BoughtByUserId { get; private set; }

    public void Apply(StarGiftSentEvent aggregateEvent)
    {
        GiftId = aggregateEvent.GiftId;
        FromUserId = aggregateEvent.FromUserId;
        ToUserId = aggregateEvent.ToUserId;
        ToPeerId = aggregateEvent.ToPeerId;
        MessageId = aggregateEvent.MessageId;
        Stars = aggregateEvent.Stars;
        ConvertStars = aggregateEvent.ConvertStars;
        Message = aggregateEvent.Message;
        NameHidden = aggregateEvent.NameHidden;
        Date = aggregateEvent.Date;
        CanUpgrade = aggregateEvent.CanUpgrade;
        UpgradeStars = aggregateEvent.UpgradeStars;
        GiftSticker = aggregateEvent.GiftSticker;
    }

    public void Apply(StarGiftSavedEvent aggregateEvent)
    {
        Saved = aggregateEvent.Saved;
        SavedId = aggregateEvent.SavedId;
    }

    public void Apply(StarGiftConvertedEvent aggregateEvent)
    {
        Converted = true;
        ConvertDate = aggregateEvent.ConvertDate;
        Saved = false; // Converting removes from profile
    }

    public void Apply(StarGiftInitiatedEvent aggregateEvent)
    {
        GiftId = aggregateEvent.GiftId;
        FromUserId = aggregateEvent.FromUserId;
        ToUserId = aggregateEvent.ToUserId;
        ToPeerId = aggregateEvent.ToPeerId;
        MessageId = aggregateEvent.MessageId;
        Stars = aggregateEvent.Stars;
        ConvertStars = aggregateEvent.ConvertStars;
        Message = aggregateEvent.Message;
        NameHidden = aggregateEvent.NameHidden;
        CanUpgrade = aggregateEvent.CanUpgrade;
        UpgradeStars = aggregateEvent.UpgradeStars;
        GiftSticker = aggregateEvent.GiftSticker;
        Date = aggregateEvent.Date;
    }

    public void Apply(StarGiftUpgradedEvent aggregateEvent)
    {
        Upgraded = true;
        UpgradeDate = aggregateEvent.UpgradeDate;
        UpgradeMsgId = aggregateEvent.UpgradeMsgId;
    }

    public void Apply(StarGiftTransferredEvent aggregateEvent)
    {
        ToUserId = aggregateEvent.NewOwnerId;
        // Reset some flags on transfer
        Saved = false;
        Pinned = false;
    }

    public void Apply(StarGiftPinnedToggledEvent aggregateEvent)
    {
        Pinned = aggregateEvent.Pinned;
    }

    public void Apply(StarGiftOutboxMessageCreatedEvent aggregateEvent)
    {
        // OutboxMessageId is stored in ReadModel, not in aggregate state
        // This is just to acknowledge the event
    }

    public void Apply(StarGiftInboxMessageCreatedEvent aggregateEvent)
    {
        MessageId = aggregateEvent.InboxMessageId;
    }

    public void Apply(StarGiftListedForResaleEvent aggregateEvent)
    {
        ForResale = true;
        ResaleStars = aggregateEvent.ResaleStars;
        ResaleDate = aggregateEvent.ResaleDate;
    }

    public void Apply(StarGiftRemovedFromResaleEvent aggregateEvent)
    {
        ForResale = false;
        ResaleStars = null;
    }

    public void Apply(StarGiftSoldViaResaleEvent aggregateEvent)
    {
        ForResale = false;
        SoldDate = aggregateEvent.SoldDate;
        BoughtByUserId = aggregateEvent.BoughtByUserId;
        ToUserId = aggregateEvent.BoughtByUserId; // Transfer ownership
        Saved = false;
        Pinned = false;
    }

    public void LoadSnapshot(StarGiftSnapshot snapshot)
    {
        GiftId = snapshot.GiftId;
        FromUserId = snapshot.FromUserId;
        ToUserId = snapshot.ToUserId;
        ToPeerId = snapshot.ToPeerId;
        MessageId = snapshot.MessageId;
        Stars = snapshot.Stars;
        ConvertStars = snapshot.ConvertStars;
        Message = snapshot.Message;
        NameHidden = snapshot.NameHidden;
        Saved = snapshot.Saved;
        Converted = snapshot.Converted;
        Upgraded = snapshot.Upgraded;
        Refunded = snapshot.Refunded;
        CanUpgrade = snapshot.CanUpgrade;
        Pinned = snapshot.Pinned;
        SavedId = snapshot.SavedId;
        UpgradeStars = snapshot.UpgradeStars;
        UpgradeMsgId = snapshot.UpgradeMsgId;
        Date = snapshot.Date;
        ConvertDate = snapshot.ConvertDate;
        UpgradeDate = snapshot.UpgradeDate;
        GiftSticker = snapshot.GiftSticker;
        IsDeleted = snapshot.IsDeleted;
    }
}
