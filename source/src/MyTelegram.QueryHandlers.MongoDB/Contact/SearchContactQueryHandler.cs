namespace MyTelegram.QueryHandlers.MongoDB.Contact;

public class SearchContactQueryHandler(IQueryOnlyReadModelStore<ContactReadModel> store) : IQueryHandler<SearchContactQuery, IReadOnlyCollection<IContactReadModel>>
{
    public async Task<IReadOnlyCollection<IContactReadModel>> ExecuteQueryAsync(SearchContactQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p =>
                    (p.SelfUserId == query.SelfUserId && (p.FirstName.Contains(query.Keyword) || p.Phone.Contains(query.Keyword) || (p.LastName != null && p.LastName.Contains(query.Keyword)))),
                cancellationToken: cancellationToken);
    }
}
