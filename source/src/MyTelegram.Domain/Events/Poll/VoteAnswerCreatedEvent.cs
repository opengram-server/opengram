namespace MyTelegram.Domain.Events.Poll;

public class VoteAnswerCreatedEvent(
    long pollId,
    long voterPeerId,
    string option,
    bool correct)
    : AggregateEvent<PollAggregate, PollId>
{
    public long PollId { get; } = pollId;
    public long VoterPeerId { get; } = voterPeerId;
    public string Option { get; } = option;
    public bool Correct { get; } = correct;
}