// ReSharper disable All

using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Queries.StarGift;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.updateStarGiftPrice" />
///</summary>
internal sealed class UpdateStarGiftPriceHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestUpdateStarGiftPrice, MyTelegram.Schema.IUpdates>,
    Payments.IUpdateStarGiftPriceHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IEventBus _eventBus;

    public UpdateStarGiftPriceHandler(
        ICommandBus commandBus, 
        IQueryProcessor queryProcessor,
        IEventBus eventBus)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
        _eventBus = eventBus;
    }

    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestUpdateStarGiftPrice obj)
    {
        string giftInstanceId;
        long giftId;
        if (obj.Stargift is TInputSavedStarGiftUser userGift)
        {
            // Load gift from ReadModel to get the real aggregate ID
            // Use preferUpgraded=true to select upgraded gift for resale
            var giftReadModel = await _queryProcessor.ProcessAsync(
                new GetStarGiftByMessageIdQuery(input.UserId, userGift.MsgId, peerId: null, preferUpgraded: true));
            
            if (giftReadModel == null)
            {
                throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
            }
            
            giftInstanceId = giftReadModel.Id;
            giftId = giftReadModel.GiftId;
        }
        else
        {
            throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }

        var aggregateId = StarGiftId.Create(giftInstanceId);
        
        var command = new UpdateStarGiftPriceCommand(
            aggregateId,
            input.ToRequestInfo(),
            obj.ResellStars
        );
        
        await _commandBus.PublishAsync(command, CancellationToken.None);
        
        // Publish integration event for catalog update
        if (obj.ResellStars > 0)
        {
            // Listed for resale
            await _eventBus.PublishAsync(new StarGiftListedForResaleIntegrationEvent(
                giftInstanceId,
                giftId,
                obj.ResellStars
            ));
            Console.WriteLine($"📨 [UpdateStarGiftPrice] Published ListedForResale event for gift {giftId}");
        }
        else
        {
            // Removed from resale
            await _eventBus.PublishAsync(new StarGiftRemovedFromResaleIntegrationEvent(
                giftInstanceId,
                giftId
            ));
            Console.WriteLine($"📨 [UpdateStarGiftPrice] Published RemovedFromResale event for gift {giftId}");
        }
        
        return new TUpdates
        {
            Chats = [],
            Updates = [],
            Users = [],
            Date = CurrentDate
        };
    }
}
