namespace MyTelegram.Domain.Shared.Stars;

public class StarsTransaction
{
    public string Id { get; set; } = string.Empty;
    public long Amount { get; set; }
    public int Date { get; set; }
    public long PeerId { get; set; }
    public string PeerType { get; set; } = "peer"; // peer, appStore, playMarket, premiumBot, fragment, ads, unsupported
    public bool Refund { get; set; }
    public bool Pending { get; set; }
    public bool Failed { get; set; }
    public bool Gift { get; set; }
    public bool Reaction { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public int? TransactionDate { get; set; }
    public string? TransactionUrl { get; set; }
    public byte[]? BotPayload { get; set; }
    public int? MsgId { get; set; }
    public int? SubscriptionPeriod { get; set; }
}
