namespace MyTelegram.Domain.Events.Poll;

public class PollClosedEvent(Peer toPeer, long pollId, int closeDate) : AggregateEvent<PollAggregate, PollId>
{
    public Peer ToPeer { get; } = toPeer;
    public long PollId { get; } = pollId;
    public int CloseDate { get; } = closeDate;
}
