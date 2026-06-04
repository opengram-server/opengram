// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// Contains the URL to use to <a href="https://corefork.telegram.org/api/stars#withdrawing-stars">withdraw Telegram Star revenue</a>.
/// See <a href="https://corefork.telegram.org/constructor/payments.StarsRevenueWithdrawalUrl" />
///</summary>
[JsonDerivedType(typeof(TStarsRevenueWithdrawalUrl), nameof(TStarsRevenueWithdrawalUrl))]
public interface IStarsRevenueWithdrawalUrl : IObject
{
    ///<summary>
    /// Contains the URL to use to <a href="https://corefork.telegram.org/api/stars#withdrawing-stars">withdraw Telegram Star revenue</a>.
    ///</summary>
    string Url { get; set; }
}
