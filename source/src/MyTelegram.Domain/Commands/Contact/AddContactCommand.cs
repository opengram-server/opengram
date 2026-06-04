namespace MyTelegram.Domain.Commands.Contact;

public class AddContactCommand(
    ContactId aggregateId,
    RequestInfo requestInfo,
    long selfUserId,
    long targetUserId,
    string phone,
    string firstName,
    string? lastName,
    bool addPhonePrivacyException)
    : RequestCommand2<ContactAggregate, ContactId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool AddPhonePrivacyException { get; } = addPhonePrivacyException;
    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public string Phone { get; } = phone;
    public long SelfUserId { get; } = selfUserId;
    public long TargetUserId { get; } = targetUserId;
}