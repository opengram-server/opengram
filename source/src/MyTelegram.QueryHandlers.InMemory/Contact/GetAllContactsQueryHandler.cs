namespace MyTelegram.QueryHandlers.InMemory.Contact;

public class GetAllContactsQueryHandler(IQueryOnlyReadModelStore<ContactReadModel> store) : IQueryHandler<GetAllContactsQuery, IReadOnlyCollection<IContactReadModel>>
{
    public async Task<IReadOnlyCollection<IContactReadModel>> ExecuteQueryAsync(GetAllContactsQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => true, skip: query.Skip, limit: query.Limit, cancellationToken: cancellationToken);
    }
}