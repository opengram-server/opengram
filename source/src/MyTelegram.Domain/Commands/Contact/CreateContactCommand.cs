namespace MyTelegram.Domain.Commands.Contact;

public class CreateContactCommand(
    ContactId aggregateId,
    long selfUserId,
    long targetUserId,
    string phone,
    string firstName,
    string? lastName,
    bool addPhonePrivacyException)
    : Command<ContactAggregate, ContactId, IExecutionResult>(aggregateId)
{
    public long SelfUserId { get; } = selfUserId;
    public long TargetUserId { get; } = targetUserId;
    public string Phone { get; } = phone;
    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public bool AddPhonePrivacyException { get; } = addPhonePrivacyException;
}