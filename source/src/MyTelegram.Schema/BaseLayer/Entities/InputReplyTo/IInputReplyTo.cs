// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Contains info about a message or story to reply to.
/// See <a href="https://corefork.telegram.org/constructor/InputReplyTo" />
///</summary>
[JsonDerivedType(typeof(TInputReplyToMessage), nameof(TInputReplyToMessage))]
[JsonDerivedType(typeof(TInputReplyToStory), nameof(TInputReplyToStory))]
[JsonDerivedType(typeof(TInputReplyToMonoForum), nameof(TInputReplyToMonoForum))]
[JsonDerivedType(typeof(TInputReplyToExternalMessage), nameof(TInputReplyToExternalMessage))]
public interface IInputReplyTo : IObject
{

}
