// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/payments.ResaleStarGifts" />
///</summary>
[JsonDerivedType(typeof(TResaleStarGifts), nameof(TResaleStarGifts))]
public interface IResaleStarGifts : IObject
{
    int Flags { get; set; }
    int Count { get; set; }
    TVector<MyTelegram.Schema.IStarGift> Gifts { get; set; }
    string? NextOffset { get; set; }
    TVector<MyTelegram.Schema.IStarGiftAttribute>? Attributes { get; set; }
    long? AttributesHash { get; set; }
    TVector<MyTelegram.Schema.IChat> Chats { get; set; }
    TVector<MyTelegram.Schema.IStarGiftAttributeCounter>? Counters { get; set; }
    TVector<MyTelegram.Schema.IUser> Users { get; set; }
}
