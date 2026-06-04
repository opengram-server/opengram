namespace MyTelegram.Core;

public record DownloadDataReceivedEvent(
    string ConnectionId,
    Guid RequestId,
    uint ObjectId,
    long UserId,
    long ReqMsgId,
    int SeqNumber,
    long AuthKeyId,
    long PermAuthKeyId,
    ReadOnlyMemory<byte> Data,
    int Layer,
    long Date,
    DeviceType DeviceType,
    string ClientIp,
    long SessionId,
    long AccessHashKeyId
) : DataReceivedEvent(
    ConnectionId,
    RequestId,
    ObjectId,
    UserId,
    ReqMsgId,
    SeqNumber,
    AuthKeyId,
    PermAuthKeyId,
    Data,
    Layer,
    Date,
    DeviceType,
    ClientIp,
    SessionId,
    AccessHashKeyId
)
{
    public static DownloadDataReceivedEvent Create()
    {
        return new DownloadDataReceivedEvent(string.Empty, Guid.Empty, 0, 0, 0, 0, 0,
            0, default, 0,
            0, DeviceType.Unknown, string.Empty, 0, 0);
    }
}
