namespace MyTelegram.QueryHandlers.InMemory.Poll;

public class GetPollsQueryHandler(IQueryOnlyReadModelStore<PollReadModel> store) : IQueryHandler<GetPollsQuery, IReadOnlyCollection<IPollReadModel>>
{
    public async Task<IReadOnlyCollection<IPollReadModel>> ExecuteQueryAsync(GetPollsQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => query.PollIds.Contains(p.PollId), cancellationToken: cancellationToken);
    }
}