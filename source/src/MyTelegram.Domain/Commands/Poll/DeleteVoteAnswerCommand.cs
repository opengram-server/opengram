using MyTelegram.Domain.Aggregates.Poll;

namespace MyTelegram.Domain.Commands.Poll;

public class DeleteVoteAnswerCommand(PollId aggregateId, long pollId, long voterPeerId)
    : Command<PollAggregate, PollId, IExecutionResult>(aggregateId)
{
    public long PollId { get; } = pollId;
    public long VoterPeerId { get; } = voterPeerId;
}