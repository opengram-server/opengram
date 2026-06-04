namespace MyTelegram.Messenger.Services.Caching;

public class PaymentFormCacheItem
{
    public long GiftId { get; set; }
    public long ToUserId { get; set; }
    public long? ToPeerId { get; set; }
    public bool IncludeUpgrade { get; set; }
    public long TotalStars { get; set; }
    public string? Message { get; set; }
    public bool HideName { get; set; }
    
    // Resale fields
    public bool IsResale { get; set; }
    public string? ResaleSlug { get; set; }
    public string? AggregateId { get; set; }
    
    // Transfer field
    public bool IsTransfer { get; set; }
}
