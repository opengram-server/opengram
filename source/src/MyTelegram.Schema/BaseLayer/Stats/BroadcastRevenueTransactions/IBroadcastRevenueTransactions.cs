// ReSharper disable All

namespace MyTelegram.Schema.Stats;

///<summary>
/// <a href="https://corefork.telegram.org/api/revenue">Channel ad revenue transactions »</a>.
/// See <a href="https://corefork.telegram.org/constructor/stats.BroadcastRevenueTransactions" />
///</summary>
[JsonDerivedType(typeof(TBroadcastRevenueTransactions), nameof(TBroadcastRevenueTransactions))]
public interface IBroadcastRevenueTransactions : IObject
{
    ///<summary>
    /// Total number of transactions.
    ///</summary>
    int Count { get; set; }

    ///<summary>
    /// Transactions
    /// See <a href="https://corefork.telegram.org/type/BroadcastRevenueTransaction" />
    ///</summary>
    TVector<MyTelegram.Schema.IBroadcastRevenueTransaction> Transactions { get; set; }
}
