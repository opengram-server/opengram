namespace MyTelegram.Domain.Events.QrCode;

public class QrCodeLoginSuccessEvent(
    RequestInfo requestInfo,
    long userId) : RequestAggregateEvent2<QrCodeAggregate, QrCodeId>(requestInfo)
{
    public long UserId { get; } = userId;
}
