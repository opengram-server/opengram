namespace MyTelegram.Domain.Commands.Contact;

public class UpdateContactProfilePhotoCommand(
    ContactId aggregateId,
    RequestInfo requestInfo,
    long selfUserId,
    long targetUserId,
    long photoId,
    bool suggest,
    IPhoto? suggestPhoto)
    : RequestCommand2<ContactAggregate, ContactId, IExecutionResult>(aggregateId, requestInfo) //, IHasCorrelationId
{
    public long SelfUserId { get; } = selfUserId;
    public long TargetUserId { get; } = targetUserId;
    public long PhotoId { get; } = photoId;
    public bool Suggest { get; } = suggest;
    public IPhoto? SuggestPhoto { get; } = suggestPhoto;
}