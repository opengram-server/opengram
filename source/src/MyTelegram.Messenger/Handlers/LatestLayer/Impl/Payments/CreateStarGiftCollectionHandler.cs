using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments;
using MyTelegram.Schema;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

public class CreateStarGiftCollectionHandler : RpcResultObjectHandler<RequestCreateStarGiftCollection, IStarGiftCollection>,
    ICreateStarGiftCollectionHandler
{
    protected override Task<IStarGiftCollection> HandleCoreAsync(IRequestInput input,
        RequestCreateStarGiftCollection obj)
    {
        // TODO: Implement collection creation logic
        // For now, return a dummy collection
        
        return Task.FromResult<IStarGiftCollection>(new TStarGiftCollection
        {
            Title = obj.Title,
            Stickers = new TVector<IDocument>()
        });
    }
}
