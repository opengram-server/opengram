// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/DisallowedGiftsSettings" />
///</summary>
[JsonDerivedType(typeof(TDisallowedGiftsSettings), nameof(TDisallowedGiftsSettings))]
public interface IDisallowedGiftsSettings : IObject
{
    int Flags { get; set; }
    bool DisallowUnlimitedStargifts { get; set; }
    bool DisallowLimitedStargifts { get; set; }
    bool DisallowUniqueStargifts { get; set; }
    bool DisallowPremiumGifts { get; set; }
}
