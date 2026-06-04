// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/PaidReactionPrivacy" />
///</summary>
[JsonDerivedType(typeof(TPaidReactionPrivacyDefault), nameof(TPaidReactionPrivacyDefault))]
[JsonDerivedType(typeof(TPaidReactionPrivacyAnonymous), nameof(TPaidReactionPrivacyAnonymous))]
[JsonDerivedType(typeof(TPaidReactionPrivacyPeer), nameof(TPaidReactionPrivacyPeer))]
public interface IPaidReactionPrivacy : IObject
{

}
