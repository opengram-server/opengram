namespace MyTelegram.Domain.Sagas.States;

public class VoteState : AggregateState<VoteSaga, VoteSagaId, VoteState>,
    IApply<VoteSagaCompletedSagaEvent>
{
    public void Apply(VoteSagaCompletedSagaEvent aggregateEvent)
    {
        //throw new NotImplementedException();
    }
}