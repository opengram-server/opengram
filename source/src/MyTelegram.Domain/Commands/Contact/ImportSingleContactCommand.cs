namespace MyTelegram.Domain.Commands.Contact;

public class ImportSingleContactCommand(
    ImportedContactId aggregateId,
    RequestInfo requestInfo,
    long selfUserId,
    PhoneContact phoneContact)
    : Command<ImportedContactAggregate, ImportedContactId, IExecutionResult>(aggregateId)
{
    public PhoneContact PhoneContact { get; } = phoneContact;
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long SelfUserId { get; } = selfUserId;
}
