namespace MyTelegram.Domain.Commands.Contact;

public class ImportContactsCommand(
    ImportedContactId aggregateId,
    RequestInfo requestInfo,
    long selfUserId,
    IReadOnlyCollection<PhoneContact> phoneContacts)
    : RequestCommand2<ImportedContactAggregate, ImportedContactId, IExecutionResult>(aggregateId, requestInfo)
{
    public IReadOnlyCollection<PhoneContact> PhoneContacts { get; } = phoneContacts;
    public long SelfUserId { get; } = selfUserId;
}
