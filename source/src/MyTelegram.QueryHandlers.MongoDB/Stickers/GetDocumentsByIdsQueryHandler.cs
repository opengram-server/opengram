using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Stickers;

public class GetDocumentsByIdsQueryHandler(
    IMongoDatabase database,
    ILogger<GetDocumentsByIdsQueryHandler> logger)
    : IQueryHandler<GetDocumentsByIdsQuery, IReadOnlyCollection<IDocumentReadModel>>
{
    public async Task<IReadOnlyCollection<IDocumentReadModel>> ExecuteQueryAsync(
        GetDocumentsByIdsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Ids == null || query.Ids.Count == 0)
        {
            logger.LogWarning("*** GetDocumentsByIdsQuery: Empty or null Ids list ***");
            return Array.Empty<IDocumentReadModel>();
        }

        logger.LogWarning("*** GetDocumentsByIdsQuery: Searching for {Count} document IDs: {Ids} ***", 
            query.Ids.Count, 
            string.Join(", ", query.Ids.Take(10)));

        // Use direct MongoDB query with $in operator
        // NOTE: Collection name is "ReadModel-DocumentReadModel" (EventFlow naming convention)
        var collection = database.GetCollection<DocumentReadModel>("ReadModel-DocumentReadModel");
        var filter = Builders<DocumentReadModel>.Filter.In(x => x.DocumentId, query.Ids);
        var allDocuments = await collection.Find(filter).ToListAsync(cancellationToken);
        
        logger.LogWarning("*** GetDocumentsByIdsQuery: Found {Count} documents in MongoDB ***", allDocuments.Count);
        
        if (allDocuments.Count > 0)
        {
            foreach (var doc in allDocuments.Take(5))
            {
                logger.LogWarning("*** Found document: DocumentId={DocumentId}, MimeType={MimeType}, Size={Size} ***",
                    doc.DocumentId, doc.MimeType, doc.Size);
            }
        }
        
        return allDocuments.Cast<IDocumentReadModel>().ToList();
    }
}
