// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/StarRefProgram" />
///</summary>
[JsonDerivedType(typeof(TStarRefProgram), nameof(TStarRefProgram))]
public interface IStarRefProgram : IObject
{
    ///<summary>
    /// Flags, see <a href="https://corefork.telegram.org/mtproto/TL-combinators#conditional-fields">TL conditional fields</a>
    ///</summary>
    int Flags { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    long BotId { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    int CommissionPermille { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    int? DurationMonths { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    int? EndDate { get; set; }

    ///<summary>
    /// &nbsp;
    /// See <a href="https://corefork.telegram.org/type/StarsAmount" />
    ///</summary>
    MyTelegram.Schema.IStarsAmount? DailyRevenuePerUser { get; set; }
}
