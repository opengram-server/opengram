namespace MyTelegram.ReadModel;

public interface IStarsReadModel : IReadModel
{
    long PeerId { get; }
    long Balance { get; }
    List<StarsTransaction> Transactions { get; }
}

public class StarsTransaction
{
    public string Id { get; set; } = null!;
    public long Amount { get; set; }
    public StarsTransactionType Type { get; set; }
    public string Reason { get; set; } = null!;
    public DateTime Date { get; set; }
}

public enum StarsTransactionType
{
    TopUp,
    Purchase,
    Refund,
    Gift
}
