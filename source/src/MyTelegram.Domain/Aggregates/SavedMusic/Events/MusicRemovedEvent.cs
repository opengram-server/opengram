namespace MyTelegram.Domain.Aggregates.SavedMusic.Events;

public class MusicRemovedEvent(
    long userId,
    long documentId,
    RequestInfo requestInfo) : AggregateEvent<SavedMusicAggregate, SavedMusicId>, IHasRequestInfo
{
    public long UserId { get; } = userId;
    public long DocumentId { get; } = documentId;
    public RequestInfo RequestInfo { get; } = requestInfo;
}
