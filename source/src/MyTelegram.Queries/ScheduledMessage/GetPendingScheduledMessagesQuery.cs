namespace MyTelegram.Queries.ScheduledMessage;

/// <summary>
/// Query to get scheduled messages that are ready to be sent
/// Used by background worker to find messages that need to be delivered
/// </summary>
public record GetPendingScheduledMessagesQuery(
    int CurrentUnixTime,
    int Limit = 100
) : IQuery<IReadOnlyList<IScheduledMessageReadModel>>;
