// ReSharper disable All

using MyTelegram.Schema.Extensions;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Set an <a href="https://corefork.telegram.org/api/emoji-status">emoji status</a>
/// See <a href="https://corefork.telegram.org/method/account.updateEmojiStatus" />
///</summary>
internal sealed class UpdateEmojiStatusHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor) : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestUpdateEmojiStatus, IBool>,
    Account.IUpdateEmojiStatusHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestUpdateEmojiStatus obj)
    {
        long? emojiStatusDocumentId = null;
        int? emojiStatusValidUntil = null;
        long? emojiStatusCollectibleId = null;

        // Разбираем emoji-статус из запроса
        if (obj.EmojiStatus is TEmojiStatus emojiStatus)
        {
            emojiStatusDocumentId = emojiStatus.DocumentId;
            emojiStatusValidUntil = emojiStatus.Until;

            // Перед установкой emoji-статуса проверяем, есть ли у пользователя Premium
            var userReadModel = await queryProcessor.ProcessAsync(new GetUserByIdQuery(input.UserId));

            if (userReadModel == null)
            {
                throw new RpcException(RpcErrors.RpcErrors400.UserIdInvalid);
            }

            // Устанавливать emoji-статус (в том числе статус из подарка) могут только Premium-пользователи
            if (!userReadModel.Premium)
            {
                Console.WriteLine($"[UpdateEmojiStatusHandler] User {input.UserId} tried to set emoji status without Premium");
                throw new RpcException(new RpcError(403, "PREMIUM_ACCOUNT_REQUIRED"));
            }

            Console.WriteLine($"[UpdateEmojiStatusHandler] User {input.UserId} has Premium, allowing emoji status DocumentId={emojiStatusDocumentId}");
        }
        else if (obj.EmojiStatus is TInputEmojiStatusCollectible collectibleStatus)
        {
            // Обрабатываем коллекционный подарок в роли emoji-статуса
            Console.WriteLine($"[UpdateEmojiStatusHandler] User {input.UserId} wants to set collectible gift {collectibleStatus.CollectibleId} as emoji status");

            // Проверяем Premium
            var userReadModel = await queryProcessor.ProcessAsync(new GetUserByIdQuery(input.UserId));

            if (userReadModel == null)
            {
                throw new RpcException(RpcErrors.RpcErrors400.UserIdInvalid);
            }

            if (!userReadModel.Premium)
            {
                Console.WriteLine($"[UpdateEmojiStatusHandler] User {input.UserId} tried to set collectible emoji status without Premium");
                throw new RpcException(new RpcError(403, "PREMIUM_ACCOUNT_REQUIRED"));
            }

            // Загружаем коллекционный подарок, чтобы получить его document ID.
            // CollectibleId — это starGiftUnique.id (небольшое уникальное число),
            // поэтому ищем подарок по полю CollectibleId.
            var giftReadModel = await queryProcessor.ProcessAsync(
                new GetStarGiftByCollectibleIdQuery(input.UserId, collectibleStatus.CollectibleId));

            if (giftReadModel == null || !giftReadModel.Upgraded)
            {
                Console.WriteLine($"[UpdateEmojiStatusHandler] Collectible gift {collectibleStatus.CollectibleId} not found or not upgraded");
                throw new RpcException(new RpcError(400, "COLLECTIBLE_INVALID"));
            }

            // Для улучшенных подарков берём document ID атрибута Model (улучшенный стикер).
            // Если атрибута Model нет, используем базовый документ стикера.
            emojiStatusDocumentId = giftReadModel.StickerDocumentId; // Значение по умолчанию

            if (giftReadModel.Attributes != null && giftReadModel.Attributes.Length > 0)
            {
                try
                {
                    var attributesVector = giftReadModel.Attributes.ToTObject<TVector<IStarGiftAttribute>>();
                    if (attributesVector != null)
                    {
                        // Ищем атрибут Model и берём из него document ID
                        foreach (var attr in attributesVector)
                        {
                            if (attr is TStarGiftAttributeModel modelAttr && modelAttr.Document is TDocument modelDoc)
                            {
                                emojiStatusDocumentId = modelDoc.Id;
                                Console.WriteLine($"[UpdateEmojiStatusHandler] Using upgraded Model document as emoji status: DocumentId={emojiStatusDocumentId}");
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UpdateEmojiStatusHandler] Failed to deserialize attributes, using base sticker: {ex.Message}");
                }
            }

            if (emojiStatusDocumentId == giftReadModel.StickerDocumentId)
            {
                Console.WriteLine($"[UpdateEmojiStatusHandler] No Model attribute found, using base sticker: DocumentId={emojiStatusDocumentId}");
            }

            emojiStatusValidUntil = collectibleStatus.Until;
            emojiStatusCollectibleId = collectibleStatus.CollectibleId;

            Console.WriteLine($"[UpdateEmojiStatusHandler] Setting collectible gift as emoji status: DocumentId={emojiStatusDocumentId}, CollectibleId={emojiStatusCollectibleId}");
        }
        else if (obj.EmojiStatus is TEmojiStatusEmpty)
        {
            // Сбрасываем emoji-статус — проверка Premium не нужна
            emojiStatusDocumentId = null;
            emojiStatusValidUntil = null;
        }

        // Отправляем команду на обновление emoji-статуса пользователя
        var command = new UpdateUserEmojiStatusCommand(
            UserId.Create(input.UserId),
            input.ToRequestInfo(),
            emojiStatusDocumentId,
            emojiStatusValidUntil,
            emojiStatusCollectibleId);

        await commandBus.PublishAsync(command, CancellationToken.None);

        return new TBoolTrue();
    }
}
