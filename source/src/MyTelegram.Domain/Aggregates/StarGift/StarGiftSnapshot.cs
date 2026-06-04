namespace MyTelegram.Domain.Aggregates.StarGift;

public class StarGiftSnapshot : ISnapshot
{
    public long GiftId { get; set; }
    public long FromUserId { get; set; }
    public long ToUserId { get; set; }
    public long? ToPeerId { get; set; }
    public int MessageId { get; set; }
    public long Stars { get; set; }
    public long ConvertStars { get; set; }
    public string? Message { get; set; }
    public bool NameHidden { get; set; }
    public bool Saved { get; set; }
    public bool Converted { get; set; }
    public bool Upgraded { get; set; }
    public bool Refunded { get; set; }
    public bool CanUpgrade { get; set; }
    public bool Pinned { get; set; }
    public long? SavedId { get; set; }
    public long? UpgradeStars { get; set; }
    public int? UpgradeMsgId { get; set; }
    public int Date { get; set; }
    public int? ConvertDate { get; set; }
    public int? UpgradeDate { get; set; }
    public byte[]? GiftSticker { get; set; }
    public bool IsDeleted { get; set; }
}
