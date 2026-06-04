namespace MyTelegram.ReadModel.Interfaces;

public interface IUserConfigReadModel : IReadModel
{
    long UserId { get; }
    string Key { get; }
    string Value { get; }
}