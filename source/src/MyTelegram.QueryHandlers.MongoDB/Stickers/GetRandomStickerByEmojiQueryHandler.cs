using MongoDB.Bson;
using MongoDB.Driver;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.Stickers;

public class GetRandomStickerByEmojiQueryHandler(IMongoDatabase database)
    : IQueryHandler<GetRandomStickerByEmojiQuery, long?>
{
    public async Task<long?> ExecuteQueryAsync(GetRandomStickerByEmojiQuery query,
        CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<StickerSetReadModel>("ReadModel-StickerSetReadModel");

        // Filter for:
        // 1. IsFeatured = true
        // 2. StickerSetType = Regular (to avoid custom emoji packs)
        // 3. Packs contains an item with Emoticon == query.Emoji
        var filter = Builders<StickerSetReadModel>.Filter.Eq(x => x.IsFeatured, true) &
                     Builders<StickerSetReadModel>.Filter.Eq(x => x.StickerSetType, StickerSetType.Regular) &
                     Builders<StickerSetReadModel>.Filter.ElemMatch(x => x.Packs, x => x.Emoticon == query.Emoji);

        // Use aggregation to:
        // 1. Match sets
        // 2. Sample 1 random set
        // 3. Project only the Packs field (optimization)
        var result = await collection.Aggregate()
            .Match(filter)
            .Sample(1)
            .Project(x => new { x.Packs })
            .FirstOrDefaultAsync(cancellationToken);

        if (result != null)
        {
            // Find the specific pack item for the emoji
            var packItem = result.Packs.FirstOrDefault(p => p.Emoticon == query.Emoji);
            
            // If exact match not found (shouldn't happen due to filter, but safety check), try with variation selector
            if (packItem == null)
            {
                packItem = result.Packs.FirstOrDefault(p => p.Emoticon == query.Emoji + "\uFE0F");
            }

            if (packItem != null && packItem.Documents.Count > 0)
            {
                // Pick a random document from the list
                var randomDocId = packItem.Documents[Random.Shared.Next(packItem.Documents.Count)];
                return randomDocId;
            }
        }

        return null;
    }
}
