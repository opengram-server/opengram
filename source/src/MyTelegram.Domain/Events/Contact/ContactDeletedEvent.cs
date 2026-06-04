namespace MyTelegram.Domain.Events.Contact;

public class ContactDeletedEvent(
    RequestInfo requestInfo,
    long targetUid) : RequestAggregateEvent2<ContactAggregate, ContactId>(requestInfo)
{
    public long TargetUid { get; } = targetUid;
}
