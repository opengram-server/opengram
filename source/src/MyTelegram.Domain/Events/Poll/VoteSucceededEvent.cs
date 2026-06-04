namespace MyTelegram.Domain.Events.Poll;

public class VoteSucceededEvent(
    RequestInfo requestInfo,
    long pollId,
    long voteUserPeerId,
    IReadOnlyCollection<string> options,
    IReadOnlyCollection<PollAnswer> answers,
    IReadOnlyCollection<string>? correctAnswers,
    IReadOnlyCollection<PollAnswerVoter> answerVoters,
    Peer toPeer,
    IReadOnlyCollection<string>? retractVoteOptions)
    : RequestAggregateEvent2<PollAggregate, PollId>(requestInfo)
{
    public long PollId { get; } = pollId;
    public long VoteUserPeerId { get; } = voteUserPeerId;
    public IReadOnlyCollection<string> Options { get; } = options;
    public IReadOnlyCollection<PollAnswer> Answers { get; } = answers;
    public IReadOnlyCollection<string>? CorrectAnswers { get; } = correctAnswers;
    public IReadOnlyCollection<PollAnswerVoter> AnswerVoters { get; } = answerVoters;
    public Peer ToPeer { get; } = toPeer;
    public IReadOnlyCollection<string>? RetractVoteOptions { get; } = retractVoteOptions;
}