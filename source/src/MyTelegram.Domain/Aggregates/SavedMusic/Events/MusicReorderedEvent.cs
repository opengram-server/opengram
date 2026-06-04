namespace MyTelegram.Domain.Aggregates.SavedMusic.Events;

public class MusicReorderedEvent(
    long userId,
    long documentId,
    long afterDocumentId,
    RequestInfo requestInfo) : AggregateEvent<SavedMusicAggregate, SavedMusicId>, IHasRequestInfo
{
    public long UserId { get; } = userId;
    public long DocumentId { get; } = documentId;
    public long AfterDocumentId { get; } = afterDocumentId;
    public RequestInfo RequestInfo { get; } = requestInfo;
}
