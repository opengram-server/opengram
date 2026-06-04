namespace MyTelegram.QueryHandlers.InMemory.Contact;

public class
    GetContactsByUidQueryHandler(IQueryOnlyReadModelStore<ContactReadModel> store) : IQueryHandler<GetContactsByUserIdQuery, IReadOnlyCollection<IContactReadModel>>
{
    public async Task<IReadOnlyCollection<IContactReadModel>> ExecuteQueryAsync(GetContactsByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.SelfUserId == query.UserId, cancellationToken: cancellationToken);
    }
}
