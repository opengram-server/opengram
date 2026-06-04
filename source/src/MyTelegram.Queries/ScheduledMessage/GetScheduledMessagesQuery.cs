namespace MyTelegram.Queries.ScheduledMessage;

/// <summary>
/// Query to get specific scheduled messages by their IDs
/// </summary>
public record GetScheduledMessagesQuery(
    long OwnerPeerId,
    List<int> ScheduleMessageIds
) : IQuery<IReadOnlyList<IScheduledMessageReadModel>>;
