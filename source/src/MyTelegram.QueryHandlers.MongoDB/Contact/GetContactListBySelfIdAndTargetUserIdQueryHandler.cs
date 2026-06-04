namespace MyTelegram.QueryHandlers.MongoDB.Contact;

public class GetContactListBySelfIdAndTargetUserIdQueryHandler(IQueryOnlyReadModelStore<ContactReadModel> store) :
    IQueryHandler<
    GetContactListBySelfIdAndTargetUserIdQuery, IReadOnlyCollection<IContactReadModel>>
{
    public async Task<IReadOnlyCollection<IContactReadModel>> ExecuteQueryAsync(GetContactListBySelfIdAndTargetUserIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p =>
            (p.SelfUserId == query.SelfUserId && p.TargetUserId == query.TargetUserId) ||
            (p.SelfUserId == query.TargetUserId && p.TargetUserId == query.SelfUserId), cancellationToken: cancellationToken);
    }
}