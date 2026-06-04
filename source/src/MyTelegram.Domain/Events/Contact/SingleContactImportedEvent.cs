namespace MyTelegram.Domain.Events.Contact;

public class SingleContactImportedEvent(
    RequestInfo requestInfo,
    long selfUserId,
    PhoneContact phoneContact)
    : RequestAggregateEvent2<ImportedContactAggregate, ImportedContactId>(requestInfo)
{
    public PhoneContact PhoneContact { get; } = phoneContact;

    public long SelfUserId { get; } = selfUserId;
}
