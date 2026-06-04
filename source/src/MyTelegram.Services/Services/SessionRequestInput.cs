namespace MyTelegram.Services.Services;

public record SessionRequestInput(
    string ConnectionId,
    ConnectionType? ConnectionType,
    Guid RequestId,
    uint ObjectId,
    long ReqMsgId,
    long UserId,
    long AuthKeyId,
    long PermAuthKeyId,
    string ClientIp,
    long RequestSessionId,
    int SeqNumber,
    bool IsAuthKeyActive,
    ReadOnlyMemory<byte> AuthKeyData,
    long ServerSalt,
    int Layer,
    long Date,
    DeviceType DeviceType,
    long AccessHashKeyId
) : RequestInput(
    ConnectionId,
    RequestId,
    ObjectId,
    ReqMsgId,
    SeqNumber,
    UserId,
    AuthKeyId,
    PermAuthKeyId,
    Layer,
    Date,
    DeviceType,
    ClientIp,
    RequestSessionId,
    AccessHashKeyId
);