namespace MyTelegram.ReadModel.Interfaces;

public interface IStarsReadModel : IReadModel
{
    long PeerId { get; }
    long Balance { get; }
    List<StarsTransaction> History { get; }
}

public class StarsTransaction
{
    public string Id { get; set; }
    public long Amount { get; set; }
    public int Date { get; set; }
    public string TransactionId { get; set; }
    public string Reason { get; set; }
}
