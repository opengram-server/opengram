using MyTelegram.Schema;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.getStarGiftWithdrawalUrl" />
///</summary>
internal sealed class GetStarGiftWithdrawalUrlHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetStarGiftWithdrawalUrl, MyTelegram.Schema.Payments.IStarGiftWithdrawalUrl>,
    Payments.IGetStarGiftWithdrawalUrlHandler
{
    protected override Task<MyTelegram.Schema.Payments.IStarGiftWithdrawalUrl> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetStarGiftWithdrawalUrl obj)
    {
        // TODO: Generate actual withdrawal URL with token
        // For now, return a placeholder URL pointing to Fragment
        
        // obj.Stargift is IInputSavedStarGift. We need to cast it or handle it.
        // Assuming TInputSavedStarGiftUser for now to get MsgId which acts as ID?
        // Or if it's just for peer ID, we might not need it in the URL if we have owner.
        // Let's just use a placeholder ID for now to satisfy compilation.
        long giftId = 0;
        if (obj.Stargift is TInputSavedStarGiftUser userGift)
        {
            giftId = userGift.MsgId;
        }

        return Task.FromResult<MyTelegram.Schema.Payments.IStarGiftWithdrawalUrl>(new TStarGiftWithdrawalUrl
        {
            Url = $"https://fragment.com/gift/export?gift_id={giftId}&owner={input.UserId}"
        });
    }
}
