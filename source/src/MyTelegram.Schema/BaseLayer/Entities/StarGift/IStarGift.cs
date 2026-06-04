// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Represents a <a href="https://corefork.telegram.org/api/gifts">star gift, see here »</a> for more info.
/// See <a href="https://corefork.telegram.org/constructor/StarGift" />
///</summary>
[JsonDerivedType(typeof(TStarGift), nameof(TStarGift))]
[JsonDerivedType(typeof(TStarGiftUnique), nameof(TStarGiftUnique))]
public interface IStarGift : IObject
{
    int Flags { get; set; }
    long Id { get; set; }
}
