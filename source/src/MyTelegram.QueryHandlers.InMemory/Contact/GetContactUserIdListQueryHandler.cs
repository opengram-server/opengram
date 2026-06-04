namespace MyTelegram.QueryHandlers.InMemory.Contact;

public class GetContactUserIdListQueryHandler(IQueryOnlyReadModelStore<ContactReadModel> store) : IQueryHandler<GetContactUserIdListQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetContactUserIdListQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.SelfUserId == query.SelfUserId, createResult: p => p.TargetUserId, cancellationToken: cancellationToken);
    }
}