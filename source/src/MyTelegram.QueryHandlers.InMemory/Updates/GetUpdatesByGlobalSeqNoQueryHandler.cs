namespace MyTelegram.QueryHandlers.InMemory.Updates;

public class GetUpdatesByGlobalSeqNoQueryHandler(IQueryOnlyReadModelStore<UpdatesReadModel> store)
    : IQueryHandler<GetUpdatesByGlobalSeqNoQuery, IReadOnlyCollection<IUpdatesReadModel>>
{
    public async Task<IReadOnlyCollection<IUpdatesReadModel>> ExecuteQueryAsync(GetUpdatesByGlobalSeqNoQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.OwnerPeerId == query.UserId &&
                                          p.UpdatesType == UpdatesType.Updates &&
                                          p.Pts == 0 &&
                                          p.GlobalSeqNo > query.MinGlobalSeqNo, cancellationToken: cancellationToken);
    }
}