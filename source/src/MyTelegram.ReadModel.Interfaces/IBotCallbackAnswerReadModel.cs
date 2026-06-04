namespace MyTelegram.ReadModel.Interfaces;

public interface IBotCallbackAnswerReadModel : IReadModel
{
    long PeerId { get; }
    bool Alert { get; }
    long QueryId { get; }
    string? Message { get; }
    string? Url { get; }
    int CacheTime { get; }
}