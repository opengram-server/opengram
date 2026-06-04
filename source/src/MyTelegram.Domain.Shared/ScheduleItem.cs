namespace MyTelegram;

public record ScheduleItem(long UserId, Peer ToPeer, int MessageId, int ScheduleDate, long? GroupId);