namespace MyTelegram.ReadModel.Interfaces;

public interface IBotMenuReadModel : IReadModel
{
    long BotUserId { get; }
    long UserId { get; }
    IBotMenuButton Menu { get; }
}