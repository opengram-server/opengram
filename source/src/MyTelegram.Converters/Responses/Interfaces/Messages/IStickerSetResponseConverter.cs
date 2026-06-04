using IStickerSet = MyTelegram.Schema.Messages.IStickerSet;
using TStickerSet = MyTelegram.Schema.Messages.TStickerSet;

namespace MyTelegram.Converters.Responses.Interfaces.Messages;

public interface IStickerSetResponseConverter
    : IResponseConverter<
        TStickerSet,
        IStickerSet
    >
{
}