// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// See <a href="https://corefork.telegram.org/constructor/SavedStarGift" />
///</summary>
[JsonDerivedType(typeof(TSavedStarGift), nameof(TSavedStarGift))]
public interface ISavedStarGift : IObject
{
    int Flags { get; set; }
    bool NameHidden { get; set; }
    bool Unsaved { get; set; }
    bool Refunded { get; set; }
    bool CanUpgrade { get; set; }
    bool PinnedToTop { get; set; }
    MyTelegram.Schema.IPeer? FromId { get; set; }
    int Date { get; set; }
    MyTelegram.Schema.IStarGift Gift { get; set; }
    MyTelegram.Schema.ITextWithEntities? Message { get; set; }
    int? MsgId { get; set; }
    long? SavedId { get; set; }
    long? ConvertStars { get; set; }
    long? UpgradeStars { get; set; }
    int? CanExportAt { get; set; }
    long? TransferStars { get; set; }
    int? CanTransferAt { get; set; }
    int? CanResellAt { get; set; }
}
