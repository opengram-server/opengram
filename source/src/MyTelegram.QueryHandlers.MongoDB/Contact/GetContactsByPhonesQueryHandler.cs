namespace MyTelegram.QueryHandlers.MongoDB.Contact;

public class
    GetContactsByPhonesQueryHandler(IQueryOnlyReadModelStore<ContactReadModel> store) : IQueryHandler<GetContactsByPhonesQuery, IReadOnlyCollection<IContactReadModel>>
{
    public async Task<IReadOnlyCollection<IContactReadModel>> ExecuteQueryAsync(GetContactsByPhonesQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.SelfUserId == query.SelfUserId && query.Phones.Contains(p.Phone), cancellationToken: cancellationToken);
    }
}