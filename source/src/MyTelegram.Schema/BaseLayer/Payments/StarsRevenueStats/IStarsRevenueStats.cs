// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// <a href="https://corefork.telegram.org/api/stars">Star revenue statistics, see here »</a> for more info.Note that all balances and currency amounts and graph values are in Stars.
/// See <a href="https://corefork.telegram.org/constructor/payments.StarsRevenueStats" />
///</summary>
[JsonDerivedType(typeof(TStarsRevenueStats), nameof(TStarsRevenueStats))]
public interface IStarsRevenueStats : IObject
{
    int Flags { get; set; }
    MyTelegram.Schema.IStatsGraph? TopHoursGraph { get; set; }

    ///<summary>
    /// Star revenue graph (number of earned stars)
    /// See <a href="https://corefork.telegram.org/type/StatsGraph" />
    ///</summary>
    MyTelegram.Schema.IStatsGraph RevenueGraph { get; set; }

    ///<summary>
    /// Current balance, current withdrawable balance and overall earned Telegram Stars
    /// See <a href="https://corefork.telegram.org/type/StarsRevenueStatus" />
    ///</summary>
    MyTelegram.Schema.IStarsRevenueStatus Status { get; set; }

    ///<summary>
    /// Current conversion rate of Telegram Stars to USD
    ///</summary>
    double UsdRate { get; set; }
}
