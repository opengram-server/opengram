using MyTelegram.Domain.Shared.Stars;

namespace MyTelegram.Queries.Stars;

public class GetStarsTransactionsQuery : IQuery<StarsStatus?>
{
    public long PeerId { get; set; }
    public bool IsInbound { get; set; }
    public bool IsOutbound { get; set; }
    public bool IsAscending { get; set; }
    public bool IsTon { get; set; }
    public string? SubscriptionId { get; set; }
    public string Offset { get; set; } = string.Empty;
    public int Limit { get; set; } = 100;
}
