namespace MyTelegram.Domain.Events.QrCode;

public class LoginTokenAcceptedEvent(
    RequestInfo requestInfo,
    long qrCodeLoginRequestTempAuthKeyId,
    long qrCodeLoginRequestPermAuthKeyId,
    byte[] token,
    long userId)
    : RequestAggregateEvent2<QrCodeAggregate, QrCodeId>(requestInfo)
{
    public long QrCodeLoginRequestPermAuthKeyId { get; } = qrCodeLoginRequestPermAuthKeyId;
    public long QrCodeLoginRequestTempAuthKeyId { get; } = qrCodeLoginRequestTempAuthKeyId;
    public byte[] Token { get; } = token;
    public long UserId { get; } = userId;
}
