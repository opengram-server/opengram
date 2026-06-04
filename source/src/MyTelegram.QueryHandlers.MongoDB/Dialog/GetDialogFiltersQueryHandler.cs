namespace MyTelegram.QueryHandlers.MongoDB.Dialog;

public class
    GetDialogFiltersQueryHandler(IQueryOnlyReadModelStore<DialogFilterReadModel> store) : IQueryHandler<GetDialogFiltersQuery, IReadOnlyCollection<IDialogFilterReadModel>>
{
    public async Task<IReadOnlyCollection<IDialogFilterReadModel>> ExecuteQueryAsync(GetDialogFiltersQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.OwnerUserId == query.OwnerUserId, cancellationToken: cancellationToken);
    }
}
