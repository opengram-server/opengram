namespace MyTelegram.Domain.Commands.Pts;

public class IncrementQtsCommand(
    PtsId aggregateId,
    RequestInfo requestInfo,
    string encryptedMessageBoxId)
    : RequestCommand2<PtsAggregate, PtsId, IExecutionResult>(aggregateId, requestInfo)
{
    public string EncryptedMessageBoxId { get; } = encryptedMessageBoxId;
}
