namespace MyTelegram.Domain.Events.Contact;

public class ContactCreatedEvent(
    long selfUserId,
    long targetUserId,
    string phone,
    string firstName,
    string? lastName,
    bool addPhonePrivacyException)
    : AggregateEvent<ContactAggregate, ContactId>
{
    public bool AddPhonePrivacyException { get; } = addPhonePrivacyException;
    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public string Phone { get; } = phone;
    public long SelfUserId { get; } = selfUserId;
    public long TargetUserId { get; } = targetUserId;
}