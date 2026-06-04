namespace MyTelegram.Domain.Shared.Stars;

public class StarsSubscription
{
    public string Id { get; set; } = string.Empty;
    public bool Canceled { get; set; }
    public bool CanRefulfill { get; set; }
    public bool MissingBalance { get; set; }
    public bool BotCanceled { get; set; }
    public long PeerId { get; set; }
    public int UntilDate { get; set; }
    public long PricingAmount { get; set; }
    public int PricingPeriod { get; set; }
    public string? ChatInviteHash { get; set; }
    public string? Title { get; set; }
    public string? PhotoUrl { get; set; }
    public string? InvoiceSlug { get; set; }
}
