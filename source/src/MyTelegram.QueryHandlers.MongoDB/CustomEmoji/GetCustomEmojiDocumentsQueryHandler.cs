using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Schema;

namespace MyTelegram.QueryHandlers.MongoDB.CustomEmoji;

public class GetCustomEmojiDocumentsQueryHandler : 
    IQueryHandler<GetCustomEmojiDocumentsQuery, IReadOnlyList<ICustomEmojiReadModel>>
{
    private readonly IQueryOnlyReadModelStore<DocumentReadModel> _documentStore;
    private readonly ILogger<GetCustomEmojiDocumentsQueryHandler> _logger;

    public GetCustomEmojiDocumentsQueryHandler(
        IQueryOnlyReadModelStore<DocumentReadModel> documentStore,
        ILogger<GetCustomEmojiDocumentsQueryHandler> logger)
    {
        _documentStore = documentStore;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ICustomEmojiReadModel>> ExecuteQueryAsync(
        GetCustomEmojiDocumentsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.DocumentIds == null || query.DocumentIds.Count == 0)
        {
            return Array.Empty<ICustomEmojiReadModel>();
        }

        _logger.LogWarning(
            "*** Getting {Count} custom emoji documents for user {UserId}: {DocumentIds} ***",
            query.DocumentIds.Count,
            query.RequesterId,
            string.Join(", ", query.DocumentIds));

        // Log all documents in collection for debugging
        try
        {
            var allDocs = await _documentStore.FindAsync(p => true, 0, 20);
            _logger.LogWarning(
                "*** Total documents in collection (first 20): {Count} ***",
                allDocs.Count);
            
            foreach (var doc in allDocs)
            {
                _logger.LogWarning(
                    "*** Document: Id={Id}, DocumentId={DocumentId}, MimeType={MimeType}, HasCustomEmojiAttr={HasAttr} ***",
                    doc.Id,
                    doc.DocumentId,
                    doc.MimeType,
                    doc.Attributes2?.Any(a => a is TDocumentAttributeCustomEmoji) ?? false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "*** Error reading all documents ***");
        }

        // Custom emojis are stored in DocumentReadModel, not in a separate collection
        var documents = await _documentStore.FindAsync(p => query.DocumentIds.Contains(p.DocumentId));
        
        _logger.LogWarning(
            "*** Found {Count} documents out of {Requested} requested ***",
            documents.Count,
            query.DocumentIds.Count);
        
        if (documents.Count == 0)
        {
            _logger.LogWarning(
                "*** NO DOCUMENTS FOUND for document IDs: {DocumentIds} ***",
                string.Join(", ", query.DocumentIds));
        }
        else
        {
            foreach (var doc in documents)
            {
                _logger.LogWarning(
                    "*** Found document: DocumentId={DocumentId}, MimeType={MimeType}, Attributes={AttrCount} ***",
                    doc.DocumentId,
                    doc.MimeType,
                    doc.Attributes2?.Count ?? 0);
            }
        }
        
        // Convert DocumentReadModel to ICustomEmojiReadModel
        var customEmojis = documents.Select(doc => new CustomEmojiAdapter(doc)).Cast<ICustomEmojiReadModel>().ToList();
        
        return customEmojis;
    }
    
    // Adapter to convert DocumentReadModel to ICustomEmojiReadModel
    private class CustomEmojiAdapter : ICustomEmojiReadModel
    {
        private readonly DocumentReadModel _document;
        private readonly TDocumentAttributeCustomEmoji? _customEmojiAttr;
        
        public CustomEmojiAdapter(IDocumentReadModel document)
        {
            _document = (DocumentReadModel)document;
            
            // Parse custom emoji attributes from Attributes2
            if (_document.Attributes2 != null)
            {
                _customEmojiAttr = _document.Attributes2
                    .OfType<TDocumentAttributeCustomEmoji>()
                    .FirstOrDefault();
            }
        }
        
        public string Id => _document.Id ?? string.Empty;
        public long Version { get; set; }
        public long DocumentId => _document.DocumentId;
        public long AccessHash => _document.AccessHash;
        public ReadOnlyMemory<byte> FileReference => _document.FileReference;
        public int DcId => _document.DcId;
        public int Date => _document.Date;
        public string MimeType => _document.MimeType;
        public long Size => _document.Size;
        public string FilePath => $"{_document.DocumentId}";
        public string? Md5CheckSum => _document.Md5CheckSum;
        
        // Parse from documentAttributeCustomEmoji if available
        public bool IsFree => _customEmojiAttr?.Free ?? true;
        public bool HasTextColor => _customEmojiAttr?.TextColor ?? false;
        public string Alt => _customEmojiAttr?.Alt ?? "🙂";
        public long StickerSetId => _customEmojiAttr?.Stickerset is TInputStickerSetID stickerSet ? stickerSet.Id : 0;
        public long? CreatorId => _document.CreatorId;
        public List<PhotoSize>? Thumbs => _document.Thumbs;
        public List<VideoSize>? VideoThumbs => _document.VideoThumbs;
        public long UsageCount => 0;
        public bool PremiumOnly => false;
    }
}
