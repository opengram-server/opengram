namespace MyTelegram.Queries.ScheduledMessage;

/// <summary>
/// Query to get scheduled message history for a peer
/// </summary>
public record GetScheduledHistoryQuery(
    long OwnerPeerId,
    int OffsetDate,
    int Limit
) : IQuery<IReadOnlyList<IScheduledMessageReadModel>>;
