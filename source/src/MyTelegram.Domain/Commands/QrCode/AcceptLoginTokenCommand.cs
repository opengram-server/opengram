namespace MyTelegram.Domain.Commands.QrCode;

public class AcceptLoginTokenCommand(
    QrCodeId aggregateId,
    RequestInfo requestInfo,
    long userId,
    byte[] token)
    : RequestCommand2<QrCodeAggregate, QrCodeId, IExecutionResult>(aggregateId, requestInfo)
{
    public byte[] Token { get; } = token;
    public long UserId { get; } = userId;
}
