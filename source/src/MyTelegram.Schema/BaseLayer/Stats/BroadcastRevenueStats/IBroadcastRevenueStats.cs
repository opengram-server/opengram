// ReSharper disable All

namespace MyTelegram.Schema.Stats;

///<summary>
/// <a href="https://corefork.telegram.org/api/revenue">Channel revenue ad statistics, see here »</a> for more info.
/// See <a href="https://corefork.telegram.org/constructor/stats.BroadcastRevenueStats" />
///</summary>
[JsonDerivedType(typeof(TBroadcastRevenueStats), nameof(TBroadcastRevenueStats))]
public interface IBroadcastRevenueStats : IObject
{
    ///<summary>
    /// Ad impressions graph
    /// See <a href="https://corefork.telegram.org/type/StatsGraph" />
    ///</summary>
    MyTelegram.Schema.IStatsGraph TopHoursGraph { get; set; }

    ///<summary>
    /// Ad revenue graph (in the smallest unit of the cryptocurrency in which revenue is calculated)
    /// See <a href="https://corefork.telegram.org/type/StatsGraph" />
    ///</summary>
    MyTelegram.Schema.IStatsGraph RevenueGraph { get; set; }

    ///<summary>
    /// Current balance, current withdrawable balance and overall revenue
    /// See <a href="https://corefork.telegram.org/type/BroadcastRevenueBalances" />
    ///</summary>
    MyTelegram.Schema.IBroadcastRevenueBalances Balances { get; set; }

    ///<summary>
    /// Current conversion rate of the cryptocurrency (<strong>not</strong> in the smallest unit) in which revenue is calculated to USD
    ///</summary>
    double UsdRate { get; set; }
}
