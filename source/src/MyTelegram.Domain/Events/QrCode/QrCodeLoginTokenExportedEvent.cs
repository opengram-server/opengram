namespace MyTelegram.Domain.Events.QrCode;

public class QrCodeLoginTokenExportedEvent(
    RequestInfo requestInfo,
    long tempAuthKeyId,
    long permAuthKeyId,
    byte[] token,
    int expireDate,
    List<long> exceptUidList)
    : RequestAggregateEvent2<QrCodeAggregate, QrCodeId>(requestInfo)
{
    public List<long> ExceptUidList { get; } = exceptUidList;
    public int ExpireDate { get; } = expireDate;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public long TempAuthKeyId { get; } = tempAuthKeyId;
    public byte[] Token { get; } = token;
}
