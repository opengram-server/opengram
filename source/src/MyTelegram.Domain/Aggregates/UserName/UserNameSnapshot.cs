namespace MyTelegram.Domain.Aggregates.UserName;

public class UserNameSnapshot(
    string? userName,
    bool isDeleted) : ISnapshot
{
    public bool IsDeleted { get; } = isDeleted;

    public string? UserName { get; } = userName;
}
