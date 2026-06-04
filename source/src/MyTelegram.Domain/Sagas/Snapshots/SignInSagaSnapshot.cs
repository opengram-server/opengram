namespace MyTelegram.Domain.Sagas.Snapshots;

public class SignInSagaSnapshot(
    long reqMsgId,
    long tempAuthKeyId,
    long permAuthKeyId)
    : ISnapshot
{
    public long PermAuthKeyId { get; } = permAuthKeyId;

    public long ReqMsgId { get; } = reqMsgId;
    public long TempAuthKeyId { get; } = tempAuthKeyId;
}
