namespace MyTelegram.QueryHandlers.MongoDB.Dialog;
//.MongoDB.Dialog

public class GetDialogByIdQueryHandler(IQueryOnlyReadModelStore<DialogReadModel> store) : IQueryHandler<GetDialogByIdQuery, IDialogReadModel?>
{
    public async Task<IDialogReadModel?> ExecuteQueryAsync(GetDialogByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);
    }
}