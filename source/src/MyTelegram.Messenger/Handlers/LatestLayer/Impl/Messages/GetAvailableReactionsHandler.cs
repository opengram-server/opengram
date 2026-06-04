// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Obtain available message reactions
/// See <a href="https://corefork.telegram.org/method/messages.getAvailableReactions" />
///</summary>
internal sealed class GetAvailableReactionsHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetAvailableReactions, MyTelegram.Schema.Messages.IAvailableReactions>,
    Messages.IGetAvailableReactionsHandler
{
    private readonly IQueryProcessor _queryProcessor;

    public GetAvailableReactionsHandler(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    protected override async Task<MyTelegram.Schema.Messages.IAvailableReactions> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetAvailableReactions obj)
    {
        // Load reactions from MongoDB reactions collection
        var reactionsQuery = new GetAvailableReactionsQuery();
        var reactionReadModels = await _queryProcessor.ProcessAsync(reactionsQuery, CancellationToken.None);

        var reactions = new List<IAvailableReaction>();

        foreach (var reaction in reactionReadModels)
        {
            // Skip inactive reactions
            if (reaction.Inactive)
            {
                continue;
            }

            // Load animation documents
            var staticIcon = await LoadDocument(reaction.StaticIconDocumentId);
            var appearAnimation = await LoadDocument(reaction.AppearAnimationDocumentId);
            var selectAnimation = await LoadDocument(reaction.SelectAnimationDocumentId);
            var activateAnimation = await LoadDocument(reaction.ActivateAnimationDocumentId);
            var effectAnimation = await LoadDocument(reaction.EffectAnimationDocumentId);
            var aroundAnimation = await LoadDocument(reaction.AroundAnimationDocumentId);
            var centerIcon = await LoadDocument(reaction.CenterIconDocumentId);

            reactions.Add(new TAvailableReaction
            {
                Reaction = reaction.Emoji,
                Title = reaction.Title,
                Premium = reaction.Premium,
                Inactive = reaction.Inactive,
                StaticIcon = staticIcon,
                AppearAnimation = appearAnimation,
                SelectAnimation = selectAnimation,
                ActivateAnimation = activateAnimation,
                EffectAnimation = effectAnimation,
                AroundAnimation = aroundAnimation,
                CenterIcon = centerIcon
            });
        }

        var hash = CalculateHash(reactions);

        // Check if client has cached version
        if (obj.Hash == hash)
        {
            return new TAvailableReactionsNotModified();
        }

        return new TAvailableReactions
        {
            Hash = hash,
            Reactions = new TVector<IAvailableReaction>(reactions)
        };
    }

    private async Task<IDocument> LoadDocument(long? documentId)
    {
        if (!documentId.HasValue || documentId.Value == 0)
        {
            return new TDocumentEmpty { Id = 0 };
        }

        var documentReadModel = await _queryProcessor.ProcessAsync(
            new GetDocumentByIdQuery(documentId.Value),
            CancellationToken.None);

        if (documentReadModel == null)
        {
            return new TDocumentEmpty { Id = documentId.Value };
        }

        return new TDocument
        {
            Id = documentReadModel.DocumentId,
            AccessHash = documentReadModel.AccessHash,
            FileReference = documentReadModel.FileReference.IsEmpty 
                ? Array.Empty<byte>() 
                : documentReadModel.FileReference.ToArray(),
            Date = documentReadModel.Date,
            MimeType = documentReadModel.MimeType,
            Size = documentReadModel.Size,
            DcId = documentReadModel.DcId,
            Attributes = documentReadModel.Attributes2 != null
                ? new TVector<IDocumentAttribute>(documentReadModel.Attributes2)
                : new TVector<IDocumentAttribute>()
        };
    }

    private int CalculateHash(List<IAvailableReaction> reactions)
    {
        if (reactions.Count == 0) return 0;
        
        var firstReaction = reactions[0] as TAvailableReaction;
        return reactions.Count * 31 + (firstReaction?.Reaction?.GetHashCode() ?? 0);
    }
}
