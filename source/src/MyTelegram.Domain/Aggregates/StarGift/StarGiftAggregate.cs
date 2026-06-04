using EventFlow.Aggregates;
using MyTelegram.Domain.Events.StarGift;

namespace MyTelegram.Domain.Aggregates.StarGift;

public class StarGiftAggregate : AggregateRoot<StarGiftAggregate, StarGiftId>
{
    private readonly StarGiftState _state = new();

    public StarGiftAggregate(StarGiftId id) : base(id)
    {
        Register(_state);
    }
    
    public void InitiateStarGift(
        RequestInfo requestInfo,
        long giftId,
        long fromUserId,
        long toUserId,
        long? toPeerId,
        int messageId,
        long stars,
        long convertStars,
        string? message,
        bool nameHidden,
        bool canUpgrade,
        long? upgradeStars,
        byte[]? giftSticker,
        int date)
    {
        Emit(new StarGiftInitiatedEvent(
            requestInfo,
            giftId,
            fromUserId,
            toUserId,
            toPeerId,
            messageId,
            stars,
            convertStars,
            message,
            nameHidden,
            canUpgrade,
            upgradeStars,
            giftSticker,
            date,
            Id.Value
        ));
    }

    public void UpgradeStarGift(RequestInfo requestInfo, int upgradeMsgId, int upgradeDate, string uniqueId, byte[]? attributes)
    {
        Emit(new StarGiftUpgradedEvent(requestInfo, upgradeMsgId, upgradeDate, uniqueId, attributes));
    }

    public void TransferStarGift(RequestInfo requestInfo, long newOwnerId, int transferDate)
    {
        Emit(new StarGiftTransferredEvent(requestInfo, newOwnerId, transferDate));
    }

    public void SaveStarGift(RequestInfo requestInfo, bool save, long? savedId)
    {
        // TODO: Emit event
    }

    public void ConvertStarGift(RequestInfo requestInfo, int convertDate)
    {
        // TODO: Emit event
    }

    public void UpdateStarGiftPrice(RequestInfo requestInfo, long price)
    {
        // TODO: Emit event
    }

    public void CompleteStarGift(RequestInfo requestInfo)
    {
        // When payment is successful, emit StarGiftSentEvent
        Emit(new StarGiftSentEvent(
            requestInfo,
            _state.GiftId,
            _state.FromUserId,
            _state.ToUserId,
            _state.ToPeerId,
            _state.MessageId,
            _state.Stars,
            _state.ConvertStars,
            _state.Message,
            _state.NameHidden,
            _state.CanUpgrade,
            _state.UpgradeStars,
            _state.GiftSticker,
            _state.Date
        ));
    }

    public void SetOutboxMessageId(int outboxMessageId)
    {
        Emit(new StarGiftOutboxMessageCreatedEvent(outboxMessageId));
    }

    public void SetInboxMessageId(int inboxMessageId)
    {
        Emit(new StarGiftInboxMessageCreatedEvent(inboxMessageId));
    }

    public void ListForResale(RequestInfo requestInfo, long resaleStars, int resaleDate)
    {
        if (!_state.Upgraded)
        {
            throw new InvalidOperationException("Only upgraded (collectible) gifts can be listed for resale");
        }

        if (_state.Converted)
        {
            throw new InvalidOperationException("Converted gifts cannot be listed for resale");
        }

        Emit(new StarGiftListedForResaleEvent(requestInfo, resaleStars, resaleDate));
    }

    public void RemoveFromResale(RequestInfo requestInfo, int removeDate)
    {
        Emit(new StarGiftRemovedFromResaleEvent(requestInfo, removeDate));
    }

    public void SellViaResale(RequestInfo requestInfo, long buyerUserId, long resaleStars, int soldDate)
    {
        Emit(new StarGiftSoldViaResaleEvent(requestInfo, buyerUserId, resaleStars, soldDate));
    }

    public void PurchaseResaleGift(RequestInfo requestInfo, long buyerUserId, long recipientUserId, long priceStars, int date)
    {
        if (!_state.ForResale)
        {
            throw new InvalidOperationException("Gift is not listed for resale");
        }

        if (!_state.Upgraded)
        {
            throw new InvalidOperationException("Only upgraded gifts can be purchased via resale");
        }

        // Emit the sold event
        Emit(new StarGiftSoldViaResaleEvent(requestInfo, buyerUserId, priceStars, date));
        
        // If buyer != recipient, this is a gift purchase (buyer gifts to someone else)
        // If buyer == recipient, this is a direct purchase for self
        if (buyerUserId != recipientUserId)
        {
            // Transfer to the recipient
            Emit(new StarGiftTransferredEvent(requestInfo, recipientUserId, date));
        }
        else
        {
            // Transfer to buyer (self-purchase)
            Emit(new StarGiftTransferredEvent(requestInfo, buyerUserId, date));
        }
    }
}
