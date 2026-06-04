// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Indicates a group call
/// See <a href="https://corefork.telegram.org/constructor/InputGroupCall" />
///</summary>
[JsonDerivedType(typeof(TInputGroupCall), nameof(TInputGroupCall))]
[JsonDerivedType(typeof(TInputGroupCallSlug), nameof(TInputGroupCallSlug))]
[JsonDerivedType(typeof(TInputGroupCallInviteMessage), nameof(TInputGroupCallInviteMessage))]
public interface IInputGroupCall : IObject
{

}
