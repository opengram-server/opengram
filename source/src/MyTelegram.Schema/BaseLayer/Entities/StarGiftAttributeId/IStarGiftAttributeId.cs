// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/StarGiftAttributeId" />
///</summary>
[JsonDerivedType(typeof(TStarGiftAttributeIdModel), nameof(TStarGiftAttributeIdModel))]
[JsonDerivedType(typeof(TStarGiftAttributeIdPattern), nameof(TStarGiftAttributeIdPattern))]
[JsonDerivedType(typeof(TStarGiftAttributeIdBackdrop), nameof(TStarGiftAttributeIdBackdrop))]
public interface IStarGiftAttributeId : IObject
{

}
