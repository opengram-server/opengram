namespace MyTelegram.Domain.Commands.QrCode;

public class LoginWithTokenSuccessCommand(QrCodeId aggregateId, RequestInfo requestInfo)
    : RequestCommand2<QrCodeAggregate, QrCodeId, IExecutionResult>(aggregateId, requestInfo);
