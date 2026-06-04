using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Fetch <a href="https://corefork.telegram.org/api/custom-emoji">custom emoji stickers »</a>.Returns a list of <a href="https://corefork.telegram.org/constructor/document">documents</a> with the animated custom emoji in TGS format, and a <a href="https://corefork.telegram.org/constructor/documentAttributeCustomEmoji">documentAttributeCustomEmoji</a> attribute with the original emoji and info about the emoji stickerset this custom emoji belongs to.
/// See <a href="https://corefork.telegram.org/method/messages.getCustomEmojiDocuments" />
///</summary>
internal sealed class GetCustomEmojiDocumentsHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetCustomEmojiDocuments, TVector<MyTelegram.Schema.IDocument>>,
    Messages.IGetCustomEmojiDocumentsHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<GetCustomEmojiDocumentsHandler> _logger;

    public GetCustomEmojiDocumentsHandler(
        IQueryProcessor queryProcessor,
        ILogger<GetCustomEmojiDocumentsHandler> logger)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
    }

    protected override async Task<TVector<MyTelegram.Schema.IDocument>> HandleCoreAsync(
        IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetCustomEmojiDocuments obj)
    {
        _logger.LogWarning("*** GetCustomEmojiDocuments CALLED by user {UserId} (for EMOJI STATUS or MESSAGE) ***", input.UserId);

        if (obj.DocumentId == null || obj.DocumentId.Count == 0)
        {
            _logger.LogWarning("*** Empty document_id list provided by user {UserId} ***", input.UserId);
            return new TVector<MyTelegram.Schema.IDocument>();
        }

        _logger.LogWarning(
            "*** User {UserId} requesting {Count} custom emoji documents: [{DocumentIds}] ***",
            input.UserId,
            obj.DocumentId.Count,
            string.Join(", ", obj.DocumentId));

        // Используем GetDocumentsByIdsQuery вместо GetCustomEmojiDocumentsQuery —
        // тот же запрос применяет GetStickerSetHandler.
        var documentReadModels = await _queryProcessor.ProcessAsync(
            new GetDocumentsByIdsQuery(obj.DocumentId.ToList()),
            CancellationToken.None);

        if (!documentReadModels.Any())
        {
            _logger.LogError(
                "*** NO DOCUMENTS FOUND in MongoDB for document IDs: [{DocumentIds}] - CHECK DATABASE! ***",
                string.Join(", ", obj.DocumentId));
            return new TVector<MyTelegram.Schema.IDocument>();
        }

        _logger.LogWarning("*** Found {Count}/{Requested} documents in MongoDB ***",
            documentReadModels.Count, obj.DocumentId.Count);

        // Преобразуем DocumentReadModel в TL Document
        var documents = new List<IDocument>();
        foreach (var doc in documentReadModels)
        {
            _logger.LogInformation("*** Converting document {DocumentId}: MimeType={MimeType}, Size={Size}, DcId={DcId} ***",
                doc.DocumentId, doc.MimeType, doc.Size, doc.DcId);

            var document = CreateDocumentFromReadModel(doc);
            documents.Add(document);
        }

        _logger.LogWarning(
            "*** RETURNING {Count} custom emoji documents to user {UserId} ***",
            documents.Count,
            input.UserId);

        return new TVector<MyTelegram.Schema.IDocument>(documents);
    }

    private IDocument CreateDocumentFromReadModel(IDocumentReadModel doc)
    {
        // Создаём TL Document из DocumentReadModel
        var document = new TDocument
        {
            Id = doc.DocumentId,
            AccessHash = doc.AccessHash,
            FileReference = doc.FileReference.IsEmpty ? Array.Empty<byte>() : doc.FileReference.ToArray(),
            Date = doc.Date,
            MimeType = doc.MimeType,
            Size = doc.Size,
            DcId = doc.DcId,
            Attributes = doc.Attributes2 != null 
                ? new TVector<IDocumentAttribute>(doc.Attributes2) 
                : new TVector<IDocumentAttribute>()
        };

        // Добавляем миниатюры, если они есть
        if (doc.Thumbs != null && doc.Thumbs.Count > 0)
        {
            document.Thumbs = new TVector<MyTelegram.Schema.IPhotoSize>(
                doc.Thumbs.Select(ConvertPhotoSize).ToList());
        }

        // Добавляем видео-миниатюры, если они есть
        if (doc.VideoThumbs != null && doc.VideoThumbs.Count > 0)
        {
            document.VideoThumbs = new TVector<MyTelegram.Schema.IVideoSize>(
                doc.VideoThumbs.Select(ConvertVideoSize).ToList());
        }

        return document;
    }

    private IPhotoSize ConvertPhotoSize(PhotoSize size)
    {
        return new TPhotoSize
        {
            Type = size.Type,
            W = size.W,
            H = size.H,
            Size = (int)size.Size
        };
    }

    private IVideoSize ConvertVideoSize(VideoSize size)
    {
        return new TVideoSize
        {
            Type = size.Type,
            W = size.W,
            H = size.H,
            Size = (int)size.Size
        };
    }
}
