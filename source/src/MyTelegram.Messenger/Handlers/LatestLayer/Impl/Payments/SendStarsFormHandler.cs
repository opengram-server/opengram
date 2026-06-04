using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Aggregates.Stars;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Domain.Commands.Stars;
using MyTelegram.Queries.StarGift;
using MyTelegram.Schema.Messages;
using MyTelegram.Schema;
using MyTelegram.Schema.Payments;
using global::EventFlow.Commands;
using MyTelegram.Messenger.Services.Caching;
using MyTelegram.Queries.Stars;
using global::EventFlow.Exceptions;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// Обработка платёжной формы Telegram Stars.
/// Отвечает за оплату и выдачу подарков Star Gifts.
///</summary>
internal sealed class SendStarsFormHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IIdGenerator idGenerator,
    ICacheManager<PaymentFormCacheItem> cacheManager,
    ILogger<SendStarsFormHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestSendStarsForm, MyTelegram.Schema.Payments.IPaymentResult>,
        Payments.ISendStarsFormHandler
{
    protected override async Task<MyTelegram.Schema.Payments.IPaymentResult> HandleCoreAsync(
        IRequestInput input,
        MyTelegram.Schema.Payments.RequestSendStarsForm obj)
    {
        logger.LogInformation("SendStarsForm called - UserId={UserId}, FormId={FormId}, Invoice={InvoiceType}",
            input.UserId, obj.FormId, obj.Invoice.GetType().Name);

        var cacheItem = await cacheManager.GetAsync(obj.FormId.ToString());

        // Проверяем, относится ли счёт к подарку Star Gift
        if (obj.Invoice is TInputInvoiceStarGift giftInvoice)
        {
            // Берём подарок из каталога
            var gift = await queryProcessor.ProcessAsync(
                new GetAvailableStarGiftByIdQuery(giftInvoice.GiftId),
                CancellationToken.None);

            if (gift == null)
            {
                logger.LogWarning("Gift not found: {GiftId}", giftInvoice.GiftId);
                throw new RpcException(new RpcError(400, "GIFT_NOT_FOUND"));
            }

            // Проверяем, не распродан ли подарок
            if (gift.SoldOut)
            {
                logger.LogWarning("Gift sold out: {GiftId}", giftInvoice.GiftId);
                throw new RpcException(new RpcError(400, "GIFT_SOLD_OUT"));
            }

            // Получаем информацию о получателе
            var toPeer = giftInvoice.Peer;
            long toUserId;
            long? toPeerId = null;

            if (toPeer is TInputPeerUser peerUser)
            {
                toUserId = peerUser.UserId;
            }
            else if (toPeer is TInputPeerChannel peerChannel)
            {
                toPeerId = peerChannel.ChannelId;
                toUserId = peerChannel.ChannelId; // для каналов userId равен channelId
            }
            else
            {
                logger.LogWarning("Invalid peer type: {Type}", toPeer.GetType().Name);
                throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
            }

            // Генерируем ID для сообщения о подарке
            var messageId = await idGenerator.NextIdAsync(IdType.MessageId, toUserId);

            // Считаем стоимость в звёздах
            var totalStars = gift.Stars;
            if (giftInvoice.IncludeUpgrade && gift.UpgradeStars.HasValue)
            {
                totalStars += gift.UpgradeStars.Value;
            }

            logger.LogInformation("Processing payment: {Stars} stars for gift {GiftId} to user {ToUserId}",
                totalStars, gift.GiftId, toUserId);


            // Создаём aggregate ID для подарка
            var aggregateId = StarGiftId.New;

            // Отправляем подарок (запускаем сагу).
            // canUpgrade должен быть true только если подарок можно улучшить (есть UpgradeStars)
            // И пользователь не выбрал улучшение сразу (IncludeUpgrade = false).
            var canUpgrade = gift.UpgradeStars.HasValue && !giftInvoice.IncludeUpgrade;
            
            var command = new InitiateStarGiftCommand(
                aggregateId,
                input.ToRequestInfo(),
                gift.GiftId,
                input.UserId, // от кого
                toUserId, // кому
                toPeerId,
                messageId,
                gift.Stars,
                gift.ConvertStars,
                giftInvoice.Message?.Text,
                giftInvoice.HideName,
                canUpgrade, // true только если можно улучшить И улучшение не включается прямо сейчас
                gift.UpgradeStars,
                gift.Sticker.HasValue ? BitConverter.GetBytes(gift.Sticker.Value) : null,
                DateTime.UtcNow.ToTimestamp()
            );
            
            logger.LogInformation("Gift parameters: CanUpgrade={CanUpgrade}, IncludeUpgrade={IncludeUpgrade}",
                canUpgrade, giftInvoice.IncludeUpgrade);

            await commandBus.PublishAsync(command, CancellationToken.None);

            logger.LogInformation("Gift sent successfully: {GiftId} from {FromUserId} to {ToUserId} (MessageId: {MessageId})",
                gift.GiftId, input.UserId, toUserId, messageId);
        }
        else if (obj.Invoice is TInputInvoiceStarGiftResale resaleInvoice)
        {
            // Обрабатываем покупку подарка с перепродажи
            logger.LogInformation("Processing resale gift purchase - Slug={Slug}, FormId={FormId}", resaleInvoice.Slug, obj.FormId);
            
            string aggregateId;
            long sellerUserId;
            long priceStars;

            if (cacheItem != null && cacheItem.IsResale)
            {
                aggregateId = cacheItem.AggregateId ?? string.Empty;
                sellerUserId = cacheItem.ToUserId;
                priceStars = cacheItem.TotalStars;
                logger.LogInformation("Using cached resale info: AggregateId={AggregateId}, Seller={Seller}, Price={Price}",
                    aggregateId, sellerUserId, priceStars);
            }
            else
            {
                // Если кэш отсутствует или устарел, разбираем slug
                var slugParts = resaleInvoice.Slug.Split('-');
                if (slugParts.Length != 2 || !slugParts[0].StartsWith("Gift"))
                {
                    logger.LogWarning("Invalid slug format: {Slug}", resaleInvoice.Slug);
                    throw new RpcException(new RpcError(400, "INVALID_SLUG_FORMAT"));
                }

                if (!long.TryParse(slugParts[1], out var collectibleId))
                {
                    throw new RpcException(new RpcError(400, "INVALID_SLUG_FORMAT"));
                }

                var gift = await queryProcessor.ProcessAsync(new GetResaleStarGiftByCollectibleIdQuery(collectibleId));
                if (gift == null)
                {
                    throw new RpcException(new RpcError(400, "GIFT_NOT_FOUND"));
                }

                aggregateId = gift.AggregateId ?? gift.Id;
                sellerUserId = gift.ToUserId;
                priceStars = gift.ResaleStars ?? 0;
            }

            if (string.IsNullOrEmpty(aggregateId))
            {
                throw new RpcException(new RpcError(400, "GIFT_NOT_FOUND"));
            }

            // Определяем получателя (ToId)
            var toPeer = resaleInvoice.ToId;
            long toUserId = input.UserId; // по умолчанию — себе

            if (toPeer is TInputPeerUser peerUser)
            {
                toUserId = peerUser.UserId;
            }

            // Сначала списываем звёзды с покупателя.
            // Это нужно сделать до покупки, чтобы нельзя было купить без оплаты.
            var spendStarsCommand = new SpendStarsCommand(
                StarsId.Create(input.UserId),
                input.ToRequestInfo(),
                priceStars,
                $"resale-{aggregateId}",
                $"Purchase resale gift {aggregateId}"
            );
            
            await commandBus.PublishAsync(spendStarsCommand, CancellationToken.None);
            logger.LogInformation("Deducted {Amount} stars from buyer {BuyerId} for resale", priceStars, input.UserId);

            // Покупаем подарок
            var command = new PurchaseResaleStarGiftCommand(
                StarGiftId.Create(aggregateId),
                input.ToRequestInfo(),
                input.UserId, // покупатель
                toUserId, // получатель
                priceStars,
                DateTime.UtcNow.ToTimestamp()
            );

            await commandBus.PublishAsync(command, CancellationToken.None);

            // Начисляем звёзды продавцу.
            // Продавец должен получить звёзды от сделки перепродажи.
            if (sellerUserId > 0 && priceStars > 0)
            {
                var sellerStarsId = StarsId.Create(sellerUserId);

                async Task PublishSellerAddStarsAsync()
                {
                    var addStarsCommand = new AddStarsCommand(
                        sellerStarsId,
                        new CommandId($"command-{Guid.NewGuid().ToString().ToLowerInvariant()}"),
                        input.ToRequestInfo(),
                        priceStars,
                        $"resale-{aggregateId}",
                        $"Resale gift payment from user {input.UserId}"
                    );

                    await commandBus.PublishAsync(addStarsCommand, CancellationToken.None);
                }

                try
                {
                    await PublishSellerAddStarsAsync();
                }
                catch (DomainError)
                {
                    var starsStatus = await queryProcessor
                        .ProcessAsync(new GetStarsStatusQuery(sellerUserId), CancellationToken.None);

                    var createCmdId = new CommandId($"command-{Guid.NewGuid()}");
                    await commandBus.PublishAsync(
                        new CreateStarsAccountCommand(sellerStarsId, createCmdId, sellerUserId),
                        CancellationToken.None);

                    if (starsStatus != null && starsStatus.Balance > 0)
                    {
                        var syncCmdId = new CommandId($"command-{Guid.NewGuid()}");
                        await commandBus.PublishAsync(
                            new AddStarsCommand(
                                sellerStarsId,
                                syncCmdId,
                                input.ToRequestInfo(),
                                starsStatus.Balance,
                                Guid.NewGuid().ToString(),
                                "Sync from ReadModel"),
                            CancellationToken.None);
                    }

                    await PublishSellerAddStarsAsync();
                }

                logger.LogInformation("Added {Amount} stars to seller {SellerId} from resale", priceStars, sellerUserId);
            }

            logger.LogInformation("Resale gift purchased: AggregateId={AggregateId}, Buyer={Buyer}, Recipient={Recipient}, Seller={Seller}, Price={Price}",
                aggregateId, input.UserId, toUserId, sellerUserId, priceStars);
        }
        else if (obj.Invoice is TInputInvoiceStarGiftTransfer transferInvoice)
        {
            // Обрабатываем передачу подарка
            logger.LogInformation("Processing gift transfer - FormId={FormId}", obj.FormId);

            if (cacheItem == null || !cacheItem.IsTransfer || string.IsNullOrEmpty(cacheItem.AggregateId))
            {
                logger.LogWarning("Transfer failed: Cache item missing or invalid for FormId={FormId}", obj.FormId);
                throw new RpcException(new RpcError(400, "PAYMENT_FORM_EXPIRED"));
            }

            var command = new TransferStarGiftCommand(
                StarGiftId.Create(cacheItem.AggregateId),
                input.ToRequestInfo(),
                cacheItem.ToUserId,
                DateTime.UtcNow.ToTimestamp()
            );

            await commandBus.PublishAsync(command, CancellationToken.None);

            logger.LogInformation("Gift transferred: AggregateId={AggregateId}, From={From}, To={To}",
                cacheItem.AggregateId, input.UserId, cacheItem.ToUserId);
        }
        else if (obj.Invoice is TInputInvoiceStars starsInvoice)
        {
             // Обрабатываем пополнение баланса звёзд.
             // starsInvoice.Purpose имеет тип InputStorePaymentPurpose.

             logger.LogInformation("Processing Stars Topup: {Purpose}", starsInvoice.Purpose.GetType().Name);

             // Имитация платежа: начисляем звёзды сразу.
             // В реальной реализации это должно вызываться по колбэку платёжного провайдера.

             long amountToAdd = 0;
             string transactionId = Guid.NewGuid().ToString();

             if (starsInvoice.Purpose is TInputStorePaymentStarsTopup storePurpose)
             {
                 // Количество звёзд задано напрямую в свойстве Stars
                 amountToAdd = storePurpose.Stars;
             }
             
             if (amountToAdd > 0)
             {
                 var command = new AddStarsCommand(
                     StarsId.Create(input.UserId),
                     new CommandId($"command-{input.RequestId}"),
                     input.ToRequestInfo(),
                     amountToAdd,
                     transactionId,
                     "Topup (Mock)"
                 );
                 
                 await commandBus.PublishAsync(command, CancellationToken.None);
                 logger.LogInformation("Added {Amount} stars to user {UserId}", amountToAdd, input.UserId);
             }
        }
        else
        {
            logger.LogWarning("Unsupported invoice type: {Type}", obj.Invoice.GetType().Name);
            throw new RpcException(new RpcError(400, "INVOICE_TYPE_UNSUPPORTED"));
        }

            // Возвращаем результат платежа вместе с обновлениями
            return new TPaymentResult
            {
                Updates = new TUpdates
                {
                    Updates = new TVector<IUpdate>(),
                    Users = new TVector<IUser>(),
                    Chats = new TVector<IChat>(),
                    Date = DateTime.UtcNow.ToTimestamp(),
                    Seq = 0
                }
            };
    }
}
