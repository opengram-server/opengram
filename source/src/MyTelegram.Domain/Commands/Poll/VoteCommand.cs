namespace MyTelegram.Domain.Commands.Poll;

public class VoteCommand(
    PollId aggregateId,
    RequestInfo requestInfo,
    long voteUserPeerId,
    IReadOnlyCollection<string> options)
    : RequestCommand2<PollAggregate, PollId, IExecutionResult>(aggregateId, requestInfo)
{
    public long VoteUserPeerId { get; } = voteUserPeerId;
    public IReadOnlyCollection<string> Options { get; } = options;
}