namespace MyTelegram.Domain.Events.Contact;

public class ContactProfilePhotoChangedEvent(
    RequestInfo requestInfo,
    long selfUserId,
    long targetUserId,
    long photoId,
    bool suggest,
    IPhoto? suggestPhoto)
    : RequestAggregateEvent2<ContactAggregate, ContactId>(requestInfo)
{
    public long SelfUserId { get; } = selfUserId;
    public long TargetUserId { get; } = targetUserId;
    public long PhotoId { get; } = photoId;
    public bool Suggest { get; } = suggest;
    public IPhoto? SuggestPhoto { get; } = suggestPhoto;
}