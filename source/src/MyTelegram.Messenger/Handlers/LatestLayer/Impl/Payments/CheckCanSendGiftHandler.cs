using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

public class CheckCanSendGiftHandler : RpcResultObjectHandler<RequestCheckCanSendGift, MyTelegram.Schema.Payments.ICheckCanSendGiftResult>,
    ICheckCanSendGiftHandler
{
    protected override Task<MyTelegram.Schema.Payments.ICheckCanSendGiftResult> HandleCoreAsync(IRequestInput input,
        RequestCheckCanSendGift obj)
    {
        // TODO: Implement actual checks (balance, limits, etc.)
        // For now, return OK to unblock client testing
        
        return Task.FromResult<MyTelegram.Schema.Payments.ICheckCanSendGiftResult>(new TCheckCanSendGiftResultOk());
    }
}
