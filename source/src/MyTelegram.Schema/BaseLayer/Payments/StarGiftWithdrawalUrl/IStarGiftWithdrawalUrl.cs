// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/payments.StarGiftWithdrawalUrl" />
///</summary>
[JsonDerivedType(typeof(TStarGiftWithdrawalUrl), nameof(TStarGiftWithdrawalUrl))]
public interface IStarGiftWithdrawalUrl : IObject
{
    string Url { get; set; }
}
