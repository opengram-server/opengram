// ReSharper disable All

namespace MyTelegram.Schema.Stats;

///<summary>
/// Contains the URL to use to <a href="https://corefork.telegram.org/api/revenue#withdrawing-revenue">withdraw channel ad revenue</a>.
/// See <a href="https://corefork.telegram.org/constructor/stats.BroadcastRevenueWithdrawalUrl" />
///</summary>
[JsonDerivedType(typeof(TBroadcastRevenueWithdrawalUrl), nameof(TBroadcastRevenueWithdrawalUrl))]
public interface IBroadcastRevenueWithdrawalUrl : IObject
{
    ///<summary>
    /// A unique URL to a Fragment page where the user will be able to specify and submit the address of the TON wallet where the funds will be sent.
    ///</summary>
    string Url { get; set; }
}
