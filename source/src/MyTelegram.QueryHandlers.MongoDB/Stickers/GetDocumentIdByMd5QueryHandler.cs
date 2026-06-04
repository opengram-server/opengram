using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.Stickers;

public class GetDocumentIdByMd5QueryHandler(IQueryOnlyReadModelStore<DocumentReadModel> store)
    : IQueryHandler<GetDocumentIdByMd5Query, long?>
{
    public async Task<long?> ExecuteQueryAsync(
        GetDocumentIdByMd5Query query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(query.Md5))
        {
            return null;
        }

        var document = await store.FindAsync(p => p.Md5CheckSum == query.Md5);
        return document.FirstOrDefault()?.DocumentId;
    }
}
