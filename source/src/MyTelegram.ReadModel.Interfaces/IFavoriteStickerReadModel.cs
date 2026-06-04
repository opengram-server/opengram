namespace MyTelegram.ReadModel.Interfaces;

public interface IFavoriteStickerReadModel : IReadModel
{
    long UserId { get; }
    long StickerId { get; }
}