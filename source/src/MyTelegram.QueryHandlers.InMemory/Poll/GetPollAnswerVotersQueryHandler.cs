namespace MyTelegram.QueryHandlers.InMemory.Poll;

public class
    GetPollAnswerVotersQueryHandler(IQueryOnlyReadModelStore<PollAnswerVoterReadModel> store) : IQueryHandler<GetPollAnswerVotersQuery,
        IReadOnlyCollection<IPollAnswerVoterReadModel>>
{
    public async Task<IReadOnlyCollection<IPollAnswerVoterReadModel>> ExecuteQueryAsync(GetPollAnswerVotersQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.PollId == query.PollId && p.VoterPeerId == query.VoterPeerId, cancellationToken: cancellationToken);
    }
}