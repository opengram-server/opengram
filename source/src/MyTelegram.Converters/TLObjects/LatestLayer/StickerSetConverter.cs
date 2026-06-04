using IStickerSet = MyTelegram.Schema.IStickerSet;
using TStickerSet = MyTelegram.Schema.TStickerSet;

namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class StickerSetConverter(IObjectMapper objectMapper) : IStickerSetConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;

    public IStickerSet ToStickerSet(long selfUserId, IStickerSetReadModel readModel)
    {
        return objectMapper.Map<IStickerSetReadModel, TStickerSet>(readModel);
    }

    public Schema.Messages.IStickerSet ToMessagesStickerSet(long userId, IStickerSetReadModel stickerSetReadModel,
        List<IDocument> documents)
    {
        var keywords = stickerSetReadModel.Keywords?.Select(p => new TStickerKeyword
        {
            DocumentId = p.DocumentId,
            Keyword = new TVector<string>(p.Keyword)
        }) ?? [];
        var packs = stickerSetReadModel.Packs.Select(p => new TStickerPack
        {
            Documents = new TVector<long>(p.Documents),
            Emoticon = p.Emoticon
        });

        var set = new Schema.Messages.TStickerSet
        {
            Keywords = new TVector<IStickerKeyword>(keywords),
            Packs = new TVector<IStickerPack>(packs),
            Set = ToStickerSet(userId, stickerSetReadModel),
            Documents = new TVector<IDocument>(documents)
        };

        return set;
    }
}