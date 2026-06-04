namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// Obtain a list of Telegram Premium <a href="https://corefork.telegram.org/api/giveaways">giveaway/gift code »</a> options.
/// See <a href="https://corefork.telegram.org/method/payments.getPremiumGiftCodeOptions" />
///</summary>
internal sealed class GetPremiumGiftCodeOptionsHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetPremiumGiftCodeOptions, TVector<MyTelegram.Schema.IPremiumGiftCodeOption>>,
    Payments.IGetPremiumGiftCodeOptionsHandler
{
    private readonly ILogger<GetPremiumGiftCodeOptionsHandler> _logger;

    public GetPremiumGiftCodeOptionsHandler(ILogger<GetPremiumGiftCodeOptionsHandler> logger)
    {
        _logger = logger;
    }

    protected override Task<TVector<MyTelegram.Schema.IPremiumGiftCodeOption>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetPremiumGiftCodeOptions obj)
    {
        _logger.LogInformation("GetPremiumGiftCodeOptions called - UserId={UserId}", input.UserId);

        // Возвращаем варианты подарочного Premium на разные сроки (нужны для работы магазина)
        var options = new TVector<MyTelegram.Schema.IPremiumGiftCodeOption>(
            [
                new TPremiumGiftCodeOption
                {
                    Months = 1,
                    Currency = "USD",
                    Amount = 500,  // $5.00
                    Users = 1
                },
                new TPremiumGiftCodeOption
                {
                    Months = 3,
                    Currency = "USD",
                    Amount = 1200,  // $12.00
                    Users = 1
                },
                new TPremiumGiftCodeOption
                {
                    Months = 6,
                    Currency = "USD",
                    Amount = 2000,  // $20.00
                    Users = 1
                },
                new TPremiumGiftCodeOption
                {
                    Months = 12,
                    Currency = "USD",
                    Amount = 3500,  // $35.00
                    Users = 1
                }
            ]);
        
        _logger.LogInformation("Returning {Count} premium gift options", options.Count);
        
        return Task.FromResult(options);
    }
}
