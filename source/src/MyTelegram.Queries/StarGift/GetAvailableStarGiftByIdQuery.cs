namespace MyTelegram.Queries.StarGift;

public record GetAvailableStarGiftByIdQuery(long GiftId) : IQuery<IAvailableStarGiftReadModel?>;
