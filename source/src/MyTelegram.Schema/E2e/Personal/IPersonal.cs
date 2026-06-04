// ReSharper disable All

namespace MyTelegram.Schema.E2e;


[JsonDerivedType(typeof(TPersonalUserId), "TPersonalUserIdLayer0")]
[JsonDerivedType(typeof(TPersonalName), "TPersonalNameLayer0")]
[JsonDerivedType(typeof(TPersonalPhoneNumber), "TPersonalPhoneNumberLayer0")]
[JsonDerivedType(typeof(TPersonalContactState), "TPersonalContactStateLayer0")]
[JsonDerivedType(typeof(TPersonalEmojiNonces), "TPersonalEmojiNoncesLayer0")]
public interface IPersonal : IObject
{

}
