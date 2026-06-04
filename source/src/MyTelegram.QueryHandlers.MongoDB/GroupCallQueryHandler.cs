using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB;

// Query definition (moved from Queries.cs to avoid circular dependency)
public record GetGroupCallByIdQuery(string GroupCallId) : IQuery<IGroupCallReadModel?>;

public class GroupCallQueryHandler(IQueryOnlyReadModelStore<GroupCallReadModel> store) 
    : IQueryHandler<GetGroupCallByIdQuery, IGroupCallReadModel?>
{
    public async Task<IGroupCallReadModel?> ExecuteQueryAsync(GetGroupCallByIdQuery query, CancellationToken cancellationToken)
    {
        // query.GroupCallId is a string (aggregate ID like "groupcall_123456789")
        var results = await store.FindAsync(p => p.Id == query.GroupCallId, cancellationToken: cancellationToken);
        return results.FirstOrDefault();
    }
}
