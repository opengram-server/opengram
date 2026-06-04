using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Queries.Business;

public class GetConnectedBotsQuery(long userId) : IQuery<List<ConnectedBot>>
{
    public long UserId { get; } = userId;
}
