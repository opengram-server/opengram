namespace MyTelegram.Domain.Events.Contact;

public class ContactAddedEvent(
    RequestInfo requestInfo,
    long selfUserId,
    long targetUserId,
    string phone,
    string firstName,
    string? lastName,
    bool addPhonePrivacyException)
    : RequestAggregateEvent2<ContactAggregate, ContactId>(requestInfo)
{
    public bool AddPhonePrivacyException { get; } = addPhonePrivacyException;
    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public string Phone { get; } = phone;
    public long SelfUserId { get; } = selfUserId;
    public long TargetUserId { get; } = targetUserId;
}