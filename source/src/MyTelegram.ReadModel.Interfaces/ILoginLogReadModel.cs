namespace MyTelegram.ReadModel.Interfaces;

public interface ILoginLogReadModel : IReadModel
{
    long UserId { get; }
    string PhoneNumber { get; }
    int Count { get; }
    DateTime LastLoginTime { get; }
}
