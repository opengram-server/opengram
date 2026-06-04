using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.Converters;
using MyTelegram.Domain.Commands.User;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Устанавливает набор стикеров пользователю.
/// <para>Possible errors</para>
/// Code Type Description
/// 406 STICKERSET_INVALID The provided sticker set is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.installStickerSet" />
///</summary>
internal sealed class InstallStickerSetHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestInstallStickerSet, MyTelegram.Schema.Messages.IStickerSetInstallResult>,
    Messages.IInstallStickerSetHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<InstallStickerSetHandler> _logger;
    private readonly ICommandBus _commandBus;

    public InstallStickerSetHandler(
        IQueryProcessor queryProcessor,
        ILogger<InstallStickerSetHandler> logger,
        ICommandBus commandBus)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
        _commandBus = commandBus;
    }

    protected override async Task<MyTelegram.Schema.Messages.IStickerSetInstallResult> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestInstallStickerSet obj)
    {
        _logger.LogInformation(
            "User {UserId} installing sticker set: {StickerSet}, Archived: {Archived}",
            input.UserId,
            obj.Stickerset,
            obj.Archived);

        // Получаем идентификатор набора из InputStickerSet
        long stickerSetId = 0;
        string? shortName = null;

        switch (obj.Stickerset)
        {
            case TInputStickerSetID inputId:
                // Перехватываем установку самодельного набора Star Gifts
                if (inputId.Id == 7770001)
                {
                    _logger.LogInformation("Installing custom Star Gifts sticker set (Id=7770001) - returning success immediately");
                    return new TStickerSetInstallResultSuccess();
                }
                stickerSetId = inputId.Id;
                break;
            case TInputStickerSetShortName inputShortName:
                // Перехватываем установку самодельного набора Star Gifts
                if (inputShortName.ShortName == "custom_star_gifts")
                {
                    _logger.LogInformation("Installing custom Star Gifts sticker set (ShortName=custom_star_gifts) - returning success immediately");
                    return new TStickerSetInstallResultSuccess();
                }
                shortName = inputShortName.ShortName;
                break;
            default:
                _logger.LogWarning("Unsupported InputStickerSet type: {Type}", obj.Stickerset.GetType().Name);
                RpcErrors.RpcErrors406.StickersetInvalid.ThrowRpcError();
                break;
        }

        // Запрашиваем набор из базы данных
        var stickerSetReadModel = shortName != null
            ? await _queryProcessor.ProcessAsync(new GetStickerSetByNameQuery(shortName), CancellationToken.None)
            : await _queryProcessor.ProcessAsync(new GetStickerSetByIdQuery(stickerSetId), CancellationToken.None);

        if (stickerSetReadModel == null)
        {
            _logger.LogWarning("Sticker set not found: id={Id}, shortName={ShortName}", stickerSetId, shortName);
            RpcErrors.RpcErrors406.StickersetInvalid.ThrowRpcError();
        }

        _logger.LogInformation(
            "Sticker set installed successfully: {StickerSetId}, UserId: {UserId}",
            stickerSetReadModel!.StickerSetId,
            input.UserId);

        _logger.LogWarning(
            "*** StickerSet details: Id={Id}, ShortName={ShortName}, Type={Type}, Masks={Masks}, Emojis={Emojis}, TextColor={TextColor} ***",
            stickerSetReadModel.StickerSetId,
            stickerSetReadModel.ShortName,
            stickerSetReadModel.StickerSetType,
            stickerSetReadModel.Masks,
            stickerSetReadModel.Emojis,
            stickerSetReadModel.TextColor);

        // Отправляем команду, чтобы сохранить установку в базе данных
        var command = new InstallStickerSetCommand(
            UserId.Create(input.UserId),
            input.ToRequestInfo(),
            stickerSetReadModel.StickerSetId,
            obj.Archived,
            stickerSetReadModel.StickerSetType
        );
        
        _logger.LogWarning(
            "*** Publishing InstallStickerSetCommand: UserId={UserId}, StickerSetId={StickerSetId}, Type={Type} ***",
            input.UserId,
            stickerSetReadModel.StickerSetId,
            stickerSetReadModel.StickerSetType);
            
        await _commandBus.PublishAsync(command, CancellationToken.None);
        
        _logger.LogWarning("*** InstallStickerSetCommand published ***");

        // Получаем документы стикеров набора
        var documents = await _queryProcessor.ProcessAsync(
            new GetDocumentsByIdsQuery((IList<long>)stickerSetReadModel.StickerDocumentIds),
            CancellationToken.None);

        // Строим соответствие documentId -> эмодзи на основе Packs
        var docIdToEmoji = new Dictionary<long, string>();
        foreach (var pack in stickerSetReadModel.Packs)
        {
            foreach (var docId in pack.Documents)
            {
                docIdToEmoji[docId] = pack.Emoticon;
            }
        }

        // Конвертируем документы в TL Document
        var tlDocuments = documents.Select(doc =>
        {
            var emoji = docIdToEmoji.GetValueOrDefault(doc.DocumentId, "🙂");
            return CustomEmojiConverter.ConvertDocumentToTl(doc, stickerSetReadModel, emoji);
        }).Cast<MyTelegram.Schema.IDocument>().ToList();

        // Конвертируем Packs
        var tlPacks = stickerSetReadModel.Packs
            .Select(p => new MyTelegram.Schema.TStickerPack
            {
                Emoticon = p.Emoticon,
                Documents = new MyTelegram.Schema.TVector<long>(p.Documents)
            })
            .Cast<MyTelegram.Schema.IStickerPack>()
            .ToList();

        // Создаём TL StickerSet
        var tlSet = new MyTelegram.Schema.TStickerSet
        {
            Id = stickerSetReadModel.StickerSetId,
            AccessHash = stickerSetReadModel.AccessHash,
            Title = stickerSetReadModel.Title,
            ShortName = stickerSetReadModel.ShortName,
            Count = stickerSetReadModel.Count,
            Emojis = stickerSetReadModel.Emojis,
            TextColor = stickerSetReadModel.TextColor,
            ChannelEmojiStatus = stickerSetReadModel.ChannelEmojiStatus,
            Masks = stickerSetReadModel.Masks,
            Archived = false,
            Official = false
        };

        // Возвращаем результат успешной установки набора.
        // Возвращаем TStickerSetInstallResultSuccess, а не Archive:
        // Archive используется только когда вытесняются старые наборы.
        _logger.LogInformation("Sticker set {StickerSetId} installed successfully", stickerSetReadModel.StickerSetId);
        
        return new TStickerSetInstallResultSuccess();
    }
}
