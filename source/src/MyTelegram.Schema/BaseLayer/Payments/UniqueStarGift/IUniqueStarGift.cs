// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/payments.UniqueStarGift" />
///</summary>
[JsonDerivedType(typeof(TUniqueStarGift), nameof(TUniqueStarGift))]
public interface IUniqueStarGift : IObject
{
    MyTelegram.Schema.IStarGift Gift { get; set; }
    TVector<MyTelegram.Schema.IUser> Users { get; set; }
}
