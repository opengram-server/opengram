namespace MyTelegram.Domain.Shared.Stars;

public class StarsStatus
{
    public long Balance { get; set; }
    public List<StarsTransaction> History { get; set; } = new();
    public string? NextOffset { get; set; }
    public List<StarsSubscription> Subscriptions { get; set; } = new();
    public string? SubscriptionsNextOffset { get; set; }
    public long? SubscriptionsMissingBalance { get; set; }
}
