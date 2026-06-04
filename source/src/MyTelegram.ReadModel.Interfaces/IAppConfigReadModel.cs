namespace MyTelegram.ReadModel.Interfaces;

public interface IAppConfigReadModel : IReadModel
{
    string Key { get; }
    string Value { get; }
    string? Description { get; }
    bool Enabled { get; }
}