// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/BusinessBotRights" />
///</summary>
[JsonDerivedType(typeof(TBusinessBotRights), nameof(TBusinessBotRights))]
public interface IBusinessBotRights : IObject
{
    int Flags { get; set; }
    bool Reply { get; set; }
    bool ReadMessages { get; set; }
    bool DeleteSentMessages { get; set; }
    bool DeleteReceivedMessages { get; set; }
    bool EditName { get; set; }
    bool EditBio { get; set; }
    bool EditProfilePhoto { get; set; }
    bool EditUsername { get; set; }
    bool ViewGifts { get; set; }
    bool SellGifts { get; set; }
    bool ChangeGiftSettings { get; set; }
    bool TransferAndUpgradeGifts { get; set; }
    bool TransferStars { get; set; }
    bool ManageStories { get; set; }
}
