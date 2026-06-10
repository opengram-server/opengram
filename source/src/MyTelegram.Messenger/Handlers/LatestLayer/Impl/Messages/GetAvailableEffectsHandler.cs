// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Returns the list of available message effects.
/// See <a href="https://corefork.telegram.org/method/messages.getAvailableEffects" />
///</summary>
internal sealed class GetAvailableEffectsHandler(
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetAvailableEffects, MyTelegram.Schema.Messages.IAvailableEffects>,
    Messages.IGetAvailableEffectsHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IAvailableEffects> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetAvailableEffects obj)
    {
        var effects = await queryProcessor.ProcessAsync(new GetAvailableEffectsQuery());

        // Collect all document IDs referenced by effects
        var documentIds = effects
            .SelectMany(e => new[] { e.StaticIconId, e.EffectStickerId, e.EffectAnimationId })
            .Where(id => id.HasValue && id.Value != 0)
            .Select(id => id!.Value)
            .Concat(effects.Select(e => e.EffectStickerId).Where(id => id != 0))
            .Distinct()
            .ToList();

        // Load all documents in a single batch query
        var documentReadModels = documentIds.Count > 0
            ? await queryProcessor.ProcessAsync(
                new GetDocumentsByIdsQuery(documentIds),
                CancellationToken.None)
            : (IReadOnlyCollection<IDocumentReadModel>)Array.Empty<IDocumentReadModel>();

        // Build TDocument list for the Documents field
        var tlDocuments = new List<IDocument>();
        foreach (var docReadModel in documentReadModels)
        {
            var tDoc = new TDocument
            {
                Id = docReadModel.DocumentId,
                AccessHash = docReadModel.AccessHash,
                FileReference = docReadModel.FileReference.IsEmpty
                    ? Array.Empty<byte>()
                    : docReadModel.FileReference.ToArray(),
                Date = docReadModel.Date,
                MimeType = docReadModel.MimeType,
                Size = docReadModel.Size,
                DcId = docReadModel.DcId,
                Attributes = docReadModel.Attributes2 != null
                    ? new TVector<IDocumentAttribute>(docReadModel.Attributes2)
                    : new TVector<IDocumentAttribute>()
            };

            if (docReadModel.Thumbs != null)
            {
                tDoc.Thumbs = new TVector<IPhotoSize>(
                    docReadModel.Thumbs.Select<PhotoSize, IPhotoSize>(p =>
                        p.Type switch
                        {
                            "i" => new TPhotoStrippedSize { Type = p.Type, Bytes = p.Bytes ?? Array.Empty<byte>() },
                            "j" => new TPhotoPathSize { Type = p.Type, Bytes = p.Bytes ?? Array.Empty<byte>() },
                            _ => new TPhotoSize { H = p.H, Size = (int)p.Size, Type = p.Type, W = p.W }
                        }).ToList());
            }

            tlDocuments.Add(tDoc);
        }

        // Build effect list
        var tlEffects = effects.Select(e => (IAvailableEffect)new TAvailableEffect
        {
            Id = e.EffectId,
            Emoticon = e.Emoticon,
            StaticIconId = e.StaticIconId,
            EffectStickerId = e.EffectStickerId,
            EffectAnimationId = e.EffectAnimationId,
            PremiumRequired = e.PremiumRequired
        }).ToList();

        // Compute hash for caching
        var hash = 0;
        foreach (var e in effects)
        {
            hash = (int)((long)hash * 20261 + e.EffectId % 0x7FFFFFFF);
        }

        // Check if client has cached version
        if (obj.Hash == hash && hash != 0)
        {
            return new TAvailableEffectsNotModified();
        }

        return new TAvailableEffects
        {
            Hash = hash,
            Effects = new TVector<IAvailableEffect>(tlEffects),
            Documents = new TVector<IDocument>(tlDocuments)
        };
    }
}
