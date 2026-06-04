// ReSharper disable All

namespace MyTelegram.Schema.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/payments.SavedStarGifts" />
///</summary>
[JsonDerivedType(typeof(TSavedStarGifts), nameof(TSavedStarGifts))]
public interface ISavedStarGifts : IObject
{
    int Flags { get; set; }
    int Count { get; set; }
    bool? ChatNotificationsEnabled { get; set; }
    TVector<MyTelegram.Schema.ISavedStarGift> Gifts { get; set; }
    string? NextOffset { get; set; }
    TVector<MyTelegram.Schema.IChat> Chats { get; set; }
    TVector<MyTelegram.Schema.IUser> Users { get; set; }
}
