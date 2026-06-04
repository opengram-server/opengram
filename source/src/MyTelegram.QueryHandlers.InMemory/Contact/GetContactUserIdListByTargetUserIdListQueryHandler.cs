namespace MyTelegram.QueryHandlers.InMemory.Contact;

public class GetContactUserIdListByTargetUserIdListQueryHandler(IQueryOnlyReadModelStore<ContactReadModel> store) : IQueryHandler<GetContactUserIdListByTargetUserIdListQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetContactUserIdListByTargetUserIdListQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => query.TargetUserIdList.Contains(p.SelfUserId) && p.TargetUserId == query.SelfUserId, createResult: p => p.TargetUserId, cancellationToken: cancellationToken);
    }
}