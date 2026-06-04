namespace MyTelegram.Domain.Commands.QrCode;

public class ExportLoginTokenCommand(
    QrCodeId aggregateId,
    RequestInfo requestInfo,
    long tempAuthKeyId,
    long permAuthKeyId,
    byte[] token,
    int expireDate,
    List<long> exceptUidList)
    : RequestCommand2<QrCodeAggregate, QrCodeId, IExecutionResult>(aggregateId, requestInfo)
{
    public List<long> ExceptUidList { get; } = exceptUidList;
    public int ExpireDate { get; } = expireDate;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public long TempAuthKeyId { get; } = tempAuthKeyId;
    public byte[] Token { get; } = token;
}
