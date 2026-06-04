// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/payments.SuggestedStarRefBots" />
///</summary>
[JsonDerivedType(typeof(TSuggestedStarRefBots), nameof(TSuggestedStarRefBots))]
public interface ISuggestedStarRefBots : IObject
{
    ///<summary>
    /// Flags, see <a href="https://corefork.telegram.org/mtproto/TL-combinators#conditional-fields">TL conditional fields</a>
    ///</summary>
    int Flags { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    int Count { get; set; }

    ///<summary>
    /// &nbsp;
    /// See <a href="https://corefork.telegram.org/type/StarRefProgram" />
    ///</summary>
    TVector<MyTelegram.Schema.IStarRefProgram> SuggestedBots { get; set; }

    ///<summary>
    /// &nbsp;
    /// See <a href="https://corefork.telegram.org/type/User" />
    ///</summary>
    TVector<MyTelegram.Schema.IUser> Users { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    string? NextOffset { get; set; }
}
