using MongoDB.Driver;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Stickers;

public class GetDocumentByIdQueryHandler(IMongoDatabase database)
    : IQueryHandler<GetDocumentByIdQuery, IDocumentReadModel?>
{
    public async Task<IDocumentReadModel?> ExecuteQueryAsync(
        GetDocumentByIdQuery query,
        CancellationToken cancellationToken)
    {
        // Use direct MongoDB query
        // NOTE: Collection name is "ReadModel-DocumentReadModel" (EventFlow naming convention)
        var collection = database.GetCollection<DocumentReadModel>("ReadModel-DocumentReadModel");
        var filter = Builders<DocumentReadModel>.Filter.Eq(x => x.DocumentId, query.Id);
        var document = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return document;
    }
}
