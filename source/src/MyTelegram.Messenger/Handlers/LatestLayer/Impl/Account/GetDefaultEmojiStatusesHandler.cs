namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Возвращает список предлагаемых по умолчанию <a href="https://corefork.telegram.org/api/emoji-status">эмодзи-статусов</a>.
/// См. <a href="https://corefork.telegram.org/method/account.getDefaultEmojiStatuses" />
///</summary>
internal sealed class GetDefaultEmojiStatusesHandler(
    IQueryProcessor queryProcessor,
    ILogger<GetDefaultEmojiStatusesHandler> logger) : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetDefaultEmojiStatuses, MyTelegram.Schema.Account.IEmojiStatuses>,
    Account.IGetDefaultEmojiStatusesHandler
{
    protected override async Task<MyTelegram.Schema.Account.IEmojiStatuses> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetDefaultEmojiStatuses obj)
    {
        logger.LogWarning("*** GetDefaultEmojiStatuses called - returning ALL installed emoji packs ***");

        // Возвращаем все установленные наборы эмодзи как набор по умолчанию, чтобы клиент подгрузил их автоматически
        var installedPacks = await queryProcessor.ProcessAsync(
            new GetInstalledStickerSetsQuery(input.UserId, StickerSetType.CustomEmoji),
            CancellationToken.None);
        
        var statuses = new List<IEmojiStatus>();
        
        var addedDocumentIds = new HashSet<long>(); // отслеживаем, чтобы не добавить дубликаты
        
        if (installedPacks != null)
        {
            foreach (var installedPack in installedPacks)
            {
                try
                {
                    // Загружаем набор стикеров целиком, чтобы получить документы
                    var stickerSet = await queryProcessor.ProcessAsync(
                        new GetStickerSetByIdQuery(installedPack.StickerSetId),
                        CancellationToken.None);
                    
                    if (stickerSet?.StickerDocumentIds != null && stickerSet.StickerDocumentIds.Count > 0)
                    {
                        // Берём первый эмодзи из набора
                        var documentId = stickerSet.StickerDocumentIds[0];
                        if (addedDocumentIds.Add(documentId))
                        {
                            statuses.Add(new TEmojiStatus
                            {
                                DocumentId = documentId,
                                Until = null
                            });
                            
                            logger.LogWarning("*** Added emoji {DocumentId} from pack {PackId} ({Title}) ***", 
                                documentId, stickerSet.StickerSetId, stickerSet.Title);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "*** Failed to load sticker set {StickerSetId} ***", installedPack.StickerSetId);
                }
            }
        }
        
        // Добавляем иконки верификации: загружаем все верификации ботов и добавляем их иконки
        try
        {
            // Запрашиваем все верификации ботов из MongoDB
            var verifications = await queryProcessor.ProcessAsync(
                new GetAllBotVerificationsQuery(),
                CancellationToken.None);
            
            if (verifications != null)
            {
                foreach (var verification in verifications)
                {
                    if (addedDocumentIds.Add(verification.IconEmojiId))
                    {
                        statuses.Add(new TEmojiStatus
                        {
                            DocumentId = verification.IconEmojiId,
                            Until = null
                        });
                        
                        logger.LogWarning("*** Added verification icon {DocumentId} for {TargetType} {TargetId} ***",
                            verification.IconEmojiId, verification.TargetType, verification.TargetId);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "*** Failed to load verification icons ***");
        }
        
        logger.LogWarning("*** Returning {Count} default emoji statuses (emoji packs + verification icons) ***", statuses.Count);
        
        return new TEmojiStatuses
        {
            Hash = 0,
            Statuses = new TVector<IEmojiStatus>(statuses)
        };
    }
}
