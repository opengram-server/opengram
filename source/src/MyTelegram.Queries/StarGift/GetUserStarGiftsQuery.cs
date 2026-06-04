using MyTelegram.Schema.Payments;

namespace MyTelegram.Queries.StarGift;

public class GetUserStarGiftsQuery(long userId, string offset, int limit) : IQuery<IUserStarGifts>
{
    public long UserId { get; } = userId;
    public string Offset { get; } = offset;
    public int Limit { get; } = limit;
}
