// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/RequirementToContact" />
///</summary>
[JsonDerivedType(typeof(TRequirementToContactEmpty), nameof(TRequirementToContactEmpty))]
[JsonDerivedType(typeof(TRequirementToContactPremium), nameof(TRequirementToContactPremium))]
[JsonDerivedType(typeof(TRequirementToContactPaidMessages), nameof(TRequirementToContactPaidMessages))]
public interface IRequirementToContact : IObject
{

}
