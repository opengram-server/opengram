namespace MyTelegram.Domain.Aggregates.Contact;

public class ImportedContactSnapshot(
    long selfUserId,
    long targetUserId,
    string phone,
    string firstName,
    string? lastName)
    : ISnapshot
{
    //public bool AddPhonePrivacyException { get; }

    //,
    //bool addPhonePrivacyException
    //AddPhonePrivacyException = addPhonePrivacyException;

    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public string Phone { get; } = phone;

    public long SelfUserId { get; } = selfUserId;
    public long TargetUserId { get; } = targetUserId;
}
