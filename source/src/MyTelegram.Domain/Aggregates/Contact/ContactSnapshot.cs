namespace MyTelegram.Domain.Aggregates.Contact;

public class ContactSnapshot(
    long selfUserId,
    long targetUserId,
    string phone,
    string firstName,
    string? lastName,
    long? photoId)
    : ISnapshot
{
    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public long? PhotoId { get; } = photoId;
    public string Phone { get; } = phone;

    public long SelfUserId { get; } = selfUserId;
    public long TargetUserId { get; } = targetUserId;
}
