using MyTelegram.Schema.Messages.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Messages;

internal sealed class GetStickerSetConverter
    : IRequestConverter<
        RequestGetStickerSet,
        Schema.Messages.RequestGetStickerSet
    >, ITransientDependency
{
    public Schema.Messages.RequestGetStickerSet ToLatestLayerData(IRequestInput request, RequestGetStickerSet obj)
    {
        return new Schema.Messages.RequestGetStickerSet
        {
            Stickerset = obj.Stickerset
        };
    }
}