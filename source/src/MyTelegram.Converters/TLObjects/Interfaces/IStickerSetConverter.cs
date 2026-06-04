using IStickerSet = MyTelegram.Schema.IStickerSet;

namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IStickerSetConverter : ILayeredConverter
{
    IStickerSet ToStickerSet(long selfUserId, IStickerSetReadModel readModel);

    Schema.Messages.IStickerSet ToMessagesStickerSet(long userId, IStickerSetReadModel stickerSetReadModel,
        List<IDocument> documents);
}