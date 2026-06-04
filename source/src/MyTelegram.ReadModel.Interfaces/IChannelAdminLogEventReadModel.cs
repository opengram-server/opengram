namespace MyTelegram.ReadModel.Interfaces;

public interface IChannelAdminLogEventReadModel : IReadModel
{
    long ChannelId { get; }
    int Date { get; }
    long UserId { get; }
    AdminLogEventAction Action { get; }
    AdminLogEventActionData Data { get; }
}