using MyTelegram.Schema.Help;

namespace MyTelegram.Converters.TLObjects.LatestLayer;

public class PremiumPromoConverter : IPremiumPromoConverter, ITransientDependency
{
    
    public virtual int Layer => Layers.LayerLatest;

    public virtual IPremiumPromo ToPremiumPromo()
    {
        return new TPremiumPromo
        {
            //Currency = "USD",
            //MonthlyAmount = 399,
            StatusText =
                "By subscribing to MyTelegram Premium you agree to the MyTelegram Terms of Service and Privacy Policy.",
            StatusEntities = new TVector<IMessageEntity>(),
            Users = new TVector<IUser>(),
            VideoSections = new TVector<string>(),
            Videos = new TVector<IDocument>(),
            PeriodOptions = new TVector<IPremiumSubscriptionOption>
            {
                new TPremiumSubscriptionOption
                {
                    Current = true,
                    Amount = 399,
                    Currency = "USD",
                    Months = 1,
                    StoreProduct = "org.telegram.telegramPremium.monthly",
                    BotUrl = string.Empty
                }
            }
        };
    }
}