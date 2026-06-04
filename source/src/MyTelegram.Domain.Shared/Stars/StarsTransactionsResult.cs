namespace MyTelegram.Domain.Shared.Stars;

public class StarsTransactionsResult
{
    public List<StarsTransaction> Transactions { get; set; } = new();
    public string? NextOffset { get; set; }
}
