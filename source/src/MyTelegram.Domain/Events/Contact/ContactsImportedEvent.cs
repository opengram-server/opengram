namespace MyTelegram.Domain.Events.Contact;

public class ContactsImportedEvent(
    RequestInfo requestInfo,
    long selfUserId,
    IReadOnlyCollection<PhoneContact> phoneContacts)
    : RequestAggregateEvent2<ImportedContactAggregate, ImportedContactId>(requestInfo)
{
    public IReadOnlyCollection<PhoneContact> PhoneContacts { get; } = phoneContacts;

    public long SelfUserId { get; } = selfUserId;
}
