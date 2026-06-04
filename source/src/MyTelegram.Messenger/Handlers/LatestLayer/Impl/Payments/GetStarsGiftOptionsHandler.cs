using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

public class GetStarsGiftOptionsHandler : RpcResultObjectHandler<RequestGetStarsGiftOptions, TVector<IStarsGiftOption>>,
    Payments.IGetStarsGiftOptionsHandler
{
    protected override Task<TVector<IStarsGiftOption>> HandleCoreAsync(IRequestInput input,
        RequestGetStarsGiftOptions obj)
    {
        var options = new TVector<IStarsGiftOption>
        {
            new TStarsGiftOption { Stars = 50, Amount = 99, Currency = "USD", StoreProduct = "stars.gift.50" },
            new TStarsGiftOption { Stars = 100, Amount = 199, Currency = "USD", StoreProduct = "stars.gift.100" },
            new TStarsGiftOption { Stars = 500, Amount = 999, Currency = "USD", StoreProduct = "stars.gift.500" }
        };

        return Task.FromResult(options);
    }
}
