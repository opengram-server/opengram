using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

public class GetStarsTopupOptionsHandler : RpcResultObjectHandler<RequestGetStarsTopupOptions, TVector<IStarsTopupOption>>,
    Payments.IGetStarsTopupOptionsHandler
{
    protected override Task<TVector<IStarsTopupOption>> HandleCoreAsync(IRequestInput input,
        RequestGetStarsTopupOptions obj)
    {
        var options = new TVector<IStarsTopupOption>
        {
            new TStarsTopupOption { Stars = 50, Amount = 99, Currency = "USD", StoreProduct = "stars.50" },
            new TStarsTopupOption { Stars = 100, Amount = 199, Currency = "USD", StoreProduct = "stars.100" },
            new TStarsTopupOption { Stars = 500, Amount = 999, Currency = "USD", StoreProduct = "stars.500" },
            new TStarsTopupOption { Stars = 1000, Amount = 1999, Currency = "USD", StoreProduct = "stars.1000" },
            new TStarsTopupOption { Stars = 2500, Amount = 4999, Currency = "USD", StoreProduct = "stars.2500" }
        };

        return Task.FromResult(options);
    }
}
