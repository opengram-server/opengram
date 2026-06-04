namespace MyTelegram.Domain.Commands.Contact;

public class DeleteContactCommand(
    ContactId aggregateId,
    RequestInfo requestInfo,
    long targetUserId)
    : RequestCommand2<ContactAggregate, ContactId, IExecutionResult>(aggregateId, requestInfo)
{
    public long TargetUserId { get; } = targetUserId;
}
