// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/StarsAmount" />
///</summary>
[JsonDerivedType(typeof(TStarsAmount), nameof(TStarsAmount))]
[JsonDerivedType(typeof(TStarsTonAmount), nameof(TStarsTonAmount))]
public interface IStarsAmount : IObject
{
    long Amount { get; set; }
}
