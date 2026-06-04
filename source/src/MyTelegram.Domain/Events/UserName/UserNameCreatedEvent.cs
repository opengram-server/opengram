namespace MyTelegram.Domain.Events.UserName;

public class UserNameCreatedEvent(Peer peer, string userName, int date) : AggregateEvent<UserNameAggregate, UserNameId>
{
    public Peer Peer { get; } = peer;
    public string UserName { get; } = userName;
    public int Date { get; } = date;
}
