using MyTelegram.Domain.Shared.BotApi;

namespace MyTelegram.ReadModel.Interfaces;

public interface IBotUpdateReadModel : IReadModel
{
    long BotUserId { get; }
    long UpdateId { get; }
    BotApiUpdate Update { get; }
    DateTime CreatedAt { get; }
    bool IsDelivered { get; }
    DateTime? DeliveredAt { get; }
    string UpdateType { get; }
}
