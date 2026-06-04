namespace MyTelegram.ReadModel.Interfaces;

public interface IReadingHistoryReadModel : IReadModel
{
    string Id { get; }
    long MessageId { get; }
    long TargetPeerId { get; }
    long ReaderPeerId { get; }
    int Date { get; }
}