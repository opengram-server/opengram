// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// <a href="https://corefork.telegram.org/api/revenue">Channel ad revenue balance »</a> information.
/// See <a href="https://corefork.telegram.org/constructor/BroadcastRevenueBalances" />
///</summary>
public interface IBroadcastRevenueBalances : IObject
{
    ///<summary>
    /// Flags, see <a href="https://corefork.telegram.org/mtproto/TL-combinators#conditional-fields">TL conditional fields</a>
    ///</summary>
    int Flags { get; set; }

    ///<summary>
    /// If set, the available balance can be <a href="https://corefork.telegram.org/api/revenue#withdrawing-revenue">withdrawn »</a>.
    ///</summary>
    bool WithdrawalEnabled { get; set; }

    ///<summary>
    /// Amount of not-yet-withdrawn cryptocurrency.
    ///</summary>
    long CurrentBalance { get; set; }

    ///<summary>
    /// Amount of withdrawable cryptocurrency, out of the currently available balance (<code>available_balance &lt;= current_balance</code>).
    ///</summary>
    long AvailableBalance { get; set; }

    ///<summary>
    /// Total amount of earned cryptocurrency.
    ///</summary>
    long OverallRevenue { get; set; }
}
