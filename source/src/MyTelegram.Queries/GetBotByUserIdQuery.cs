using EventFlow.Queries;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Queries;

public class GetBotByUserIdQuery : IQuery<IBotReadModel?>
{
    public GetBotByUserIdQuery(long userId)
    {
        UserId = userId;
    }

    public long UserId { get; }
}
