// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Object defines a group.
/// See <a href="https://corefork.telegram.org/constructor/Chat" />
///</summary>
[JsonDerivedType(typeof(TChatEmpty), nameof(TChatEmpty))]
[JsonDerivedType(typeof(TChat), nameof(TChat))]
[JsonDerivedType(typeof(TChatForbidden), nameof(TChatForbidden))]
[JsonDerivedType(typeof(TChannel), nameof(TChannel))]
[JsonDerivedType(typeof(TChannelForbidden), nameof(TChannelForbidden))]
public interface IChat : IObject
{
    ///<summary>
    /// ID of the channel, see <a href="https://corefork.telegram.org/api/peers#peer-id">here »</a> for more info
    ///</summary>
    long Id { get; set; }
}
