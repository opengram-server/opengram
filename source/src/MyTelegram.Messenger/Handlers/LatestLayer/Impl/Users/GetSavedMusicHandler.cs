using MyTelegram.Schema.Users;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Users;

///<summary>
/// Get songs pinned to the user's profile
/// See <a href="https://corefork.telegram.org/method/users.getSavedMusic" />
///</summary>
internal sealed class GetSavedMusicHandler(
    IQueryProcessor queryProcessor,
    ILogger<GetSavedMusicHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Users.RequestGetSavedMusic, MyTelegram.Schema.Users.ISavedMusic>
{
    protected override async Task<MyTelegram.Schema.Users.ISavedMusic> HandleCoreAsync(
        IRequestInput input,
        RequestGetSavedMusic obj)
    {
        // Get target user ID
        long targetUserId = obj.Id switch
        {
            TInputUserSelf => input.UserId,
            TInputUser inputUser => inputUser.UserId,
            _ => throw new NotSupportedException($"Unsupported InputUser type: {obj.Id.GetType().Name}")
        };

        // Get saved music
        var savedMusic = await queryProcessor.ProcessAsync(
            new GetSavedMusicByUserIdQuery(targetUserId),
            CancellationToken.None);

        if (savedMusic == null || savedMusic.DocumentIds.Count == 0)
        {
            return new MyTelegram.Schema.Users.TSavedMusic
            {
                Count = 0,
                Documents = new TVector<IDocument>()
            };
        }

        // Apply pagination
        var documentIds = savedMusic.DocumentIds
            .Skip(obj.Offset)
            .Take(obj.Limit)
            .ToList();

        if (documentIds.Count == 0)
        {
            return new MyTelegram.Schema.Users.TSavedMusic
            {
                Count = savedMusic.DocumentIds.Count,
                Documents = new TVector<IDocument>()
            };
        }

        // Load documents
        var documents = await queryProcessor.ProcessAsync(
            new GetDocumentsByIdsQuery(documentIds),
            CancellationToken.None);

        // Convert to TL
        var tlDocuments = new List<IDocument>();
        foreach (var docId in documentIds)
        {
            var doc = documents.FirstOrDefault(d => d.DocumentId == docId);
            if (doc != null)
            {
                tlDocuments.Add(new TDocument
                {
                    Id = doc.DocumentId,
                    AccessHash = doc.AccessHash,
                    FileReference = doc.FileReference.IsEmpty ? Array.Empty<byte>() : doc.FileReference.ToArray(),
                    Date = doc.Date,
                    MimeType = doc.MimeType ?? "audio/mpeg",
                    Size = doc.Size,
                    DcId = doc.DcId,
                    Attributes = doc.Attributes2 != null
                        ? new TVector<IDocumentAttribute>(doc.Attributes2)
                        : new TVector<IDocumentAttribute>()
                });
            }
        }

        logger.LogInformation(
            "Returning {Count} saved music documents for user {UserId} (total: {Total})",
            tlDocuments.Count,
            targetUserId,
            savedMusic.DocumentIds.Count);

        return new MyTelegram.Schema.Users.TSavedMusic
        {
            Count = savedMusic.DocumentIds.Count,
            Documents = new TVector<IDocument>(tlDocuments)
        };
    }
}
