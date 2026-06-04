namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.getSavedStarGift" />
///</summary>
internal sealed class GetSavedStarGiftHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetSavedStarGift, MyTelegram.Schema.Payments.ISavedStarGifts>,
    Payments.IGetSavedStarGiftHandler
{
    protected override Task<MyTelegram.Schema.Payments.ISavedStarGifts> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetSavedStarGift obj)
    {
        throw new NotImplementedException();
    }
}
