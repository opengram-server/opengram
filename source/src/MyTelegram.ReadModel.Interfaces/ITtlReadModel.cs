namespace MyTelegram.ReadModel.Interfaces;

public interface ITtlReadModel : IReadModel
{
    long PeerId { get; }
    int? Ttl { get; }
}
