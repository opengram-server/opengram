// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/payments.StarGiftUpgradePreview" />
///</summary>
[JsonDerivedType(typeof(TStarGiftUpgradePreview), nameof(TStarGiftUpgradePreview))]
public interface IStarGiftUpgradePreview : IObject
{
    TVector<MyTelegram.Schema.IStarGiftAttribute> SampleAttributes { get; set; }
}
