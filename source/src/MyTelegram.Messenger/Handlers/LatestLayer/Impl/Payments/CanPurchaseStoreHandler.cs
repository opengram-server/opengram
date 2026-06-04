// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.canPurchaseStore" />
///</summary>
internal sealed class CanPurchaseStoreHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestCanPurchaseStore, IBool>,
    Payments.ICanPurchaseStoreHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestCanPurchaseStore obj)
    {
        throw new NotImplementedException();
    }
}
