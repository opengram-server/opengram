namespace MyTelegram.ReadModel.Interfaces;

public interface IAuthKeyReadModel : IReadModel
{
    long AuthKeyId { get; }
    ReadOnlyMemory<byte> Data { get; }
    string Id { get; }
    bool IsActive { get; }
    DateTime LastUpdateTime { get; }
    long ServerSalt { get; }
    long UserId { get; }
    int Layer { get; }
    DeviceType? DeviceType { get; }
    //long AccessHashKeyId { get; }

    long AccessHashKeyId { get; }
    
}