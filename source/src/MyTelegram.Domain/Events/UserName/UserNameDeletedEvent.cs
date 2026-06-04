namespace MyTelegram.Domain.Events.UserName;

public class UserNameDeletedEvent(Peer peer) : AggregateEvent<UserNameAggregate, UserNameId>
{
    public Peer Peer { get; } = peer;
}
