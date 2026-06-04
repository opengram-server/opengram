namespace MyTelegram.Domain.Events.Poll;

public class PollCreatedEvent(
    Peer toPeer,
    long pollId,
    bool multipleChoice,
    bool quiz,
    bool publicVoters,
    string question,
    IReadOnlyCollection<PollAnswer> answers,
    IReadOnlyCollection<string>? correctAnswers,
    string? solution,
    byte[]? solutionEntities,
    IList<IMessageEntity>? solutionEntities2,
    byte[]? questionEntities,
    IList<IMessageEntity>? questionEntities2
    )
    : AggregateEvent<PollAggregate, PollId>
{
    public Peer ToPeer { get; } = toPeer;
    public long PollId { get; } = pollId;
    public bool MultipleChoice { get; } = multipleChoice;
    public bool Quiz { get; } = quiz;
    public bool PublicVoters { get; } = publicVoters;
    public string Question { get; } = question;
    public IReadOnlyCollection<PollAnswer> Answers { get; } = answers;
    public IReadOnlyCollection<string>? CorrectAnswers { get; } = correctAnswers;
    public string? Solution { get; } = solution;
    public byte[]? SolutionEntities { get; } = solutionEntities;
    public IList<IMessageEntity>? SolutionEntities2 { get; } = solutionEntities2;
    public byte[]? QuestionEntities { get; } = questionEntities;
    public IList<IMessageEntity>? QuestionEntities2 { get; } = questionEntities2;
}