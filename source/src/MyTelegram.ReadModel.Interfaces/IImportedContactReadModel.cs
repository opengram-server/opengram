namespace MyTelegram.ReadModel.Interfaces;

public interface IImportedContactReadModel : IReadModel
{
    string FirstName { get; }
    string Id { get; }
    string? LastName { get; }
    string Phone { get; }
    long SelfUserId { get; }
    long TargetUserId { get; }
}