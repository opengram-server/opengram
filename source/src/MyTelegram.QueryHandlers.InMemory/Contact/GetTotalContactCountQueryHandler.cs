namespace MyTelegram.QueryHandlers.InMemory.Contact;

public class GetTotalContactCountQueryHandler(IQueryOnlyReadModelStore<ContactReadModel> store) : IQueryHandler<GetTotalContactCountQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetTotalContactCountQuery query,
        CancellationToken cancellationToken)
    {
        return (int)await store.CountAsync(p => true, cancellationToken);
    }
}