namespace MyTelegram.ReadModel.Interfaces;

public interface IJoinChannelRequestReadModel : IReadModel
{
    long ChannelId { get; }
    long UserId { get; }
    int Date { get; }
    long? InviteId { get; }
    bool Approved { get; }
    bool IsJoinRequestProcessed { get; }
    long? ProcessedByUserId { get; }
}