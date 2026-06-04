using IStickerSet = MyTelegram.Schema.Messages.IStickerSet;
using TStickerSet = MyTelegram.Schema.Messages.TStickerSet;

namespace MyTelegram.Converters.Responses.LatestLayer.Messages;

internal sealed class StickerSetResponseConverter : IStickerSetResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IStickerSet ToLayeredData(TStickerSet obj)
    {
        return obj;
    }
}