using EventFlow.Core;

namespace MyTelegram.Domain.Aggregates.StarGift;

public class StarGiftId : Identity<StarGiftId>
{
    public StarGiftId(string value) : base(value)
    {
    }

    public static StarGiftId Create(string value)
    {
        return new StarGiftId(value);
    }
}
