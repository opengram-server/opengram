namespace MyTelegram.QueryHandlers.MongoDB.Contact;

public class GetContactQueryHandler(IQueryOnlyReadModelStore<ContactReadModel> store) : IQueryHandler<GetContactQuery, IContactReadModel?>
{
    public async Task<IContactReadModel?> ExecuteQueryAsync(GetContactQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.SelfUserId == query.SelfUserId && p.TargetUserId == query.TargetUserId, cancellationToken);
    }
}