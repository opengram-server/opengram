namespace MyTelegram.ReadModel.Interfaces;

public interface IAccessHashReadModel : IReadModel
{
    long AccessId { get; }
    long AccessHash { get; }
    AccessHashType AccessHashType { get; }
}