using MyTelegram.Schema.Users;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Users;

///<summary>
/// Check if the passed songs are still pinned to the user's profile, or refresh the file references
/// See <a href="https://corefork.telegram.org/method/users.getSavedMusicByID" />
///</summary>
internal sealed class GetSavedMusicByIDHandler(IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Users.RequestGetSavedMusicByID, MyTelegram.Schema.Users.ISavedMusic>
{
    protected override async Task<MyTelegram.Schema.Users.ISavedMusic> HandleCoreAsync(
        IRequestInput input,
        RequestGetSavedMusicByID obj)
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

        // Extract requested document IDs
        var requestedIds = obj.Documents
            .OfType<TInputDocument>()
            .Select(d => d.Id)
            .ToList();

        // Filter only documents that are still in saved music
        var validIds = requestedIds
            .Where(id => savedMusic.DocumentIds.Contains(id))
            .ToList();

        if (validIds.Count == 0)
        {
            return new MyTelegram.Schema.Users.TSavedMusic
            {
                Count = savedMusic.DocumentIds.Count,
                Documents = new TVector<IDocument>()
            };
        }

        // Load documents
        var documents = await queryProcessor.ProcessAsync(
            new GetDocumentsByIdsQuery(validIds),
            CancellationToken.None);

        // Convert to TL (with refreshed file references)
        var tlDocuments = new List<IDocument>();
        foreach (var docId in validIds)
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

        return new MyTelegram.Schema.Users.TSavedMusic
        {
            Count = savedMusic.DocumentIds.Count,
            Documents = new TVector<IDocument>(tlDocuments)
        };
    }
}
