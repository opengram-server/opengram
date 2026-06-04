using MyTelegram.Domain.Aggregates.SavedMusic;

namespace MyTelegram.Domain.Commands.SavedMusic;

public class SaveMusicCommand(
    SavedMusicId aggregateId,
    RequestInfo requestInfo,
    long userId,
    long documentId,
    bool unsave,
    long? afterDocumentId)
    : RequestCommand2<SavedMusicAggregate, SavedMusicId, IExecutionResult>(aggregateId, requestInfo)
{
    public long UserId { get; } = userId;
    public long DocumentId { get; } = documentId;
    public bool Unsave { get; } = unsave;
    public long? AfterDocumentId { get; } = afterDocumentId;
}
