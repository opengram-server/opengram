namespace MyTelegram.ReadModel.Interfaces;

public interface IUserStickerConfigReadModel : IReadModel
{
    long UserId { get; }
    List<long> Order { get; }
}