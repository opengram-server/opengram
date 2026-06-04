namespace MyTelegram.QueryHandlers.InMemory.Poll;

public class
    GetChosenVoteAnswersQueryHandler(IQueryOnlyReadModelStore<PollAnswerVoterReadModel> store) : IQueryHandler<GetChosenVoteAnswersQuery,
        IReadOnlyCollection<IPollAnswerVoterReadModel>>
{
    public async Task<IReadOnlyCollection<IPollAnswerVoterReadModel>> ExecuteQueryAsync(GetChosenVoteAnswersQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => query.PollIds.Contains(p.PollId) && p.VoterPeerId == query.VoterPeerId, cancellationToken: cancellationToken);
    }
}