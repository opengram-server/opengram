// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/StarGiftAttributeCounter" />
///</summary>
[JsonDerivedType(typeof(TStarGiftAttributeCounter), nameof(TStarGiftAttributeCounter))]
public interface IStarGiftAttributeCounter : IObject
{
    MyTelegram.Schema.IStarGiftAttributeId Attribute { get; set; }
    int Count { get; set; }
}
