namespace MyTelegram;

public class UserItem(
    long userId,
    long accessHash,
    string phone,
    string firstName,
    string? lastName,
    string? userName)

{
    public long AccessHash { get; init; } = accessHash;
    public string FirstName { get; init; } = firstName;
    public string? LastName { get; init; } = lastName;

    public string Phone { get; init; } = phone;
    public byte[]? ProfilePhoto { get; init; }

    public long UserId { get; init; } = userId;
    public string? UserName { get; init; } = userName;
}