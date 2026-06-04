using MyTelegram.Domain.Aggregates.Poll;

namespace MyTelegram.Domain.Commands.Poll;

public class CreateVoteAnswerCommand(
    PollId aggregateId,
    long pollId,
    long voterPeerId,
    string option,
    bool correct)
    : Command<PollAggregate, PollId, IExecutionResult>(aggregateId)
{
    public long PollId { get; } = pollId;
    public long VoterPeerId { get; } = voterPeerId;
    public string Option { get; } = option;
    public bool Correct { get; } = correct;
}