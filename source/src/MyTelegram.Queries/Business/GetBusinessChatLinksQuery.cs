using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Queries.Business;

public class GetBusinessChatLinksQuery(long userId) : IQuery<List<BusinessChatLink>>
{
    public long UserId { get; } = userId;
}
