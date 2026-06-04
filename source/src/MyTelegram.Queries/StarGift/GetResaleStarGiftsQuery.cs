using MyTelegram.Schema;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Queries.StarGift;

public class GetResaleStarGiftsQuery(
    int offset, 
    int limit, 
    long? giftId = null, 
    TVector<IStarGiftAttributeId>? attributes = null, 
    bool sortByPrice = false, 
    bool sortByNum = false) : IQuery<IResaleStarGifts>
{
    public int Offset { get; } = offset;
    public int Limit { get; } = limit;
    public long? GiftId { get; } = giftId;
    public TVector<IStarGiftAttributeId>? Attributes { get; } = attributes;
    public bool SortByPrice { get; } = sortByPrice;
    public bool SortByNum { get; } = sortByNum;
}
