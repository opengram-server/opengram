// ReSharper disable All

namespace MyTelegram.Schema.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/messages.FoundStickers" />
///</summary>
[JsonDerivedType(typeof(TFoundStickersNotModified), nameof(TFoundStickersNotModified))]
[JsonDerivedType(typeof(TFoundStickers), nameof(TFoundStickers))]
public interface IFoundStickers : IObject
{
    ///<summary>
    /// Flags, see <a href="https://corefork.telegram.org/mtproto/TL-combinators#conditional-fields">TL conditional fields</a>
    ///</summary>
    int Flags { get; set; }

    ///<summary>
    /// &nbsp;
    ///</summary>
    int? NextOffset { get; set; }
}
