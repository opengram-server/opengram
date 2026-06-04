namespace MyTelegram.Domain.Sagas.Events;

public class VoteSagaCompletedSagaEvent(
    RequestInfo requestInfo,
    long pollId,
    long voterPeerId,
    IReadOnlyCollection<string> chosenOptions,
    Peer toPeer)
    : RequestAggregateEvent2<VoteSaga, VoteSagaId>(requestInfo)
{
    public long PollId { get; } = pollId;
    public long VoterPeerId { get; } = voterPeerId;
    public IReadOnlyCollection<string> ChosenOptions { get; } = chosenOptions;
    public Peer ToPeer { get; } = toPeer;
}