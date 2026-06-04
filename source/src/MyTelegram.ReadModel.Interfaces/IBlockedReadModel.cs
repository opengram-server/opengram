namespace MyTelegram.ReadModel.Interfaces;

public interface IBlockedReadModel : IReadModel
{
    int Date { get; }
    string Id { get; }
    long TargetPeerId { get; }
    long UserId { get; }
}
