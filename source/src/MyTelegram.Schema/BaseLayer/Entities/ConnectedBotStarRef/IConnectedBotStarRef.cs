// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/ConnectedBotStarRef" />
///</summary>
[JsonDerivedType(typeof(TConnectedBotStarRef), nameof(TConnectedBotStarRef))]
public interface IConnectedBotStarRef : IObject
{
    ///<summary>
    /// Flags, see <a href="https://corefork.telegram.org/mtproto/TL-combinators#conditional-fields">TL conditional fields</a>
    ///</summary>
    int Flags { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    bool Revoked { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    string Url { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    int Date { get; set; }

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
    long Participants { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    long Revenue { get; set; }
}
