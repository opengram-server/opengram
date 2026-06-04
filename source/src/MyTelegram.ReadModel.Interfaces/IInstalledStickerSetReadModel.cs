namespace MyTelegram.ReadModel.Interfaces;

public interface IInstalledStickerSetReadModel : IReadModel
{
    long UserId { get; }
    long StickerSetId { get; }
    bool Archived { get; }
    StickerSetType StickerSetType { get; }
    int Date { get; }
}