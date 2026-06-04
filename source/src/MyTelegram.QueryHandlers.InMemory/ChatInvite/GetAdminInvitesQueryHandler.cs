namespace MyTelegram.QueryHandlers.InMemory.ChatInvite;

public class GetAdminInvitesQueryHandler : IQueryHandler<GetAdminInvitesQuery, IReadOnlyCollection<AdminWithInvites>>
{
    public Task<IReadOnlyCollection<AdminWithInvites>> ExecuteQueryAsync(GetAdminInvitesQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<AdminWithInvites>>(new List<AdminWithInvites>());
    }
}