namespace MyTelegram.Domain.Aggregates.Messaging;

public class ScheduledMessageId : Identity<ScheduledMessageId>
{
    public ScheduledMessageId(string value) : base(value)
    {
    }

    public static string Create(long ownerPeerId, int scheduleMessageId)
    {
        return $"{ownerPeerId}_{scheduleMessageId}";
    }
}
