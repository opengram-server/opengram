using EventFlow.Queries;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.Messenger.QueryHandlers;

public class GetContactListBySelfUserIdQueryHandler(IQueryProcessor queryProcessor)
    : IQueryHandler<GetContactListBySelfUserIdQuery, List<ContactReadModel>>
{
    public async Task<List<ContactReadModel>> ExecuteQueryAsync(
        GetContactListBySelfUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var contacts = await queryProcessor.ProcessAsync(
            new GetContactListQuery(query.SelfUserId, []), 
            cancellationToken);
        return contacts.OfType<ContactReadModel>().ToList();
    }
}
