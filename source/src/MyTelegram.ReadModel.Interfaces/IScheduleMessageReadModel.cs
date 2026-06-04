namespace MyTelegram.ReadModel.Interfaces;

public interface IScheduleMessageReadModel : IReadModel
{
    long UserId { get; }
    long ToPeerId { get; }
    int MessageId { get; }
    SendMessageItem Item { get; }
    int ScheduleDate { get; }
}