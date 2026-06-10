using EventFlow.Queries;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Queries;

public class GetBotsByOwnerUserIdQuery(long ownerUserId) : IQuery<IReadOnlyCollection<IBotReadModel>>
{
    public long OwnerUserId { get; } = ownerUserId;
}
