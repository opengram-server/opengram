using MongoDB.Bson;
using MongoDB.Driver;
using MyTelegram.Queries.StarGift;
using MyTelegram.Queries.User;
using MyTelegram.ReadModel.Impl;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// Формирует форму оплаты для Star Gifts
///</summary>
internal sealed class GetPaymentFormHandler(
    IQueryProcessor queryProcessor,
    IIdGenerator idGenerator,
    ICacheManager<PaymentFormCacheItem> cacheManager,
    IUserConverterService userConverterService,
    IMongoDatabase mongoDatabase,
    ILogger<GetPaymentFormHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetPaymentForm, MyTelegram.Schema.Payments.IPaymentForm>,
        Payments.IGetPaymentFormHandler
{
    protected override async Task<MyTelegram.Schema.Payments.IPaymentForm> HandleCoreAsync(
        IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetPaymentForm obj)
    {
        logger.LogInformation("GetPaymentForm called - UserId={UserId}, Invoice={InvoiceType}",
            input.UserId, obj.Invoice.GetType().Name);

        // Проверяем, не счёт ли это на перепродажу Star Gift
        if (obj.Invoice is TInputInvoiceStarGiftResale resaleInvoice)
        {
            return await HandleResaleGiftPaymentForm(input, resaleInvoice);
        }

        // Проверяем, не счёт ли это на передачу Star Gift
        if (obj.Invoice is TInputInvoiceStarGiftTransfer transferInvoice)
        {
            return await HandleTransferGiftPaymentForm(input, transferInvoice);
        }

        // Проверяем, не счёт ли это на улучшение Star Gift
        if (obj.Invoice is TInputInvoiceStarGiftUpgrade upgradeInvoice)
        {
            logger.LogInformation("Star Gift Upgrade handled by UpgradeStarGiftHandler");
            throw new RpcException(new RpcError(400, "UPGRADE_USE_DEDICATED_METHOD"));
        }

        // Проверяем, что это счёт на покупку Star Gift
        if (obj.Invoice is not TInputInvoiceStarGift giftInvoice)
        {
            logger.LogWarning("Unsupported invoice type: {Type}", obj.Invoice.GetType().Name);
            throw new RpcException(new RpcError(400, "INVOICE_TYPE_UNSUPPORTED"));
        }



        // Проверяем, что peer задан и не пустой
        if (giftInvoice.Peer == null || giftInvoice.Peer is TInputPeerEmpty)
        {
            logger.LogError("Peer is empty or null in inputInvoiceStarGift - client bug! Peer type: {PeerType}",
                giftInvoice.Peer?.GetType().Name ?? "null");
            throw new RpcException(new RpcError(400, "PEER_ID_INVALID"));
        }
// Берём подарок из каталога
        var gift = await queryProcessor.ProcessAsync(
            new GetAvailableStarGiftByIdQuery(giftInvoice.GiftId),
            CancellationToken.None);

        if (gift == null)
        {
            logger.LogWarning("Gift not found: {GiftId}", giftInvoice.GiftId);
            throw new RpcException(new RpcError(400, "GIFT_NOT_FOUND"));
        }

        // Считаем итоговое количество звёзд
        var totalStars = gift.Stars;
        if (giftInvoice.IncludeUpgrade && gift.UpgradeStars.HasValue)
        {
            totalStars += gift.UpgradeStars.Value;
        }

        // Генерируем идентификатор формы
        var formId = (long)(uint)Guid.NewGuid().GetHashCode();

        // Кешируем контекст платежа.
        // Получатель берётся из поля peer счёта TInputInvoiceStarGift
        // (flags, hide_name, keep_original_details, gift_id, message, peer).
        var cacheItem = new PaymentFormCacheItem
        {
            GiftId = gift.GiftId,
            ToUserId = input.UserId,
            ToPeerId = giftInvoice.Peer switch
            {
                TInputPeerUser peerUser => peerUser.UserId,
                TInputPeerChat peerChat => peerChat.ChatId,
                TInputPeerChannel peerChannel => peerChannel.ChannelId,
                _ => throw new RpcException(new RpcError(400, "PEER_ID_INVALID"))
            }, 
            IncludeUpgrade = giftInvoice.IncludeUpgrade,
            TotalStars = totalStars,
            Message = giftInvoice.Message?.Text,
            HideName = giftInvoice.HideName
        };
        
        await cacheManager.SetAsync(formId.ToString(), cacheItem);

        logger.LogInformation("Payment form created: FormId={FormId}, Stars={Stars}, Gift={GiftId}, ToPeerId={ToPeerId}, InputPeer={InputPeerType}",
            formId, totalStars, gift.GiftId, cacheItem.ToPeerId, giftInvoice.Peer?.GetType().Name ?? "null");

        // Получаем данные получателя, если peer задан
        var users = new TVector<IUser>();
        if (cacheItem.ToPeerId.HasValue)
        {
            logger.LogInformation("Fetching recipient user: ToPeerId={ToPeerId}", cacheItem.ToPeerId.Value);
            
            var recipientUser = await queryProcessor.ProcessAsync(
                new GetUserByIdQuery(cacheItem.ToPeerId.Value),
                CancellationToken.None);
            
            if (recipientUser != null)
            {
                logger.LogInformation("Recipient user found: UserId={UserId}, AccessHash={AccessHash}",
                    recipientUser.UserId, recipientUser.AccessHash);

                // ToUser учитывает настройки приватности, но для надёжности
                // дополнительно скрываем телефон при отправке подарков.
                var layeredUser = userConverterService.ToUser(input, recipientUser);

                // Принудительно скрываем телефон получателя подарка, даже если он в контактах:
                // при отправке подарков телефон получателя показывать не нужно.
                if (layeredUser is TUser user)
                {
                    user.Phone = null;
                    user.ComputeFlag();
                    logger.LogInformation("[PRIVACY] Forced hiding phone for gift recipient {UserId}", recipientUser.UserId);
                }

                users.Add(layeredUser);

                logger.LogInformation("Added recipient to Users array, total users={Count}", users.Count);
            }
            else
            {
                logger.LogWarning("Recipient user NOT found for ToPeerId={ToPeerId}", cacheItem.ToPeerId.Value);
            }
        }
        else
        {
            logger.LogWarning("ToPeerId is null in cache item");
        }

        // Возвращаем форму оплаты для star gift (используем TPaymentFormStarGift, а не TPaymentFormStars)
        return new TPaymentFormStarGift
        {
            FormId = formId,
            Invoice = new TInvoice
            {
                Test = false,
                NameRequested = false,
                PhoneRequested = false,
                EmailRequested = false,
                ShippingAddressRequested = false,
                Flexible = false,
                PhoneToProvider = false,
                EmailToProvider = false,
                Recurring = false,
                Currency = "XTR",
                Prices = new TVector<ILabeledPrice>
                {
                    new TLabeledPrice
                    {
                        Label = gift.Title ?? "Gift",
                        Amount = totalStars
                    }
                }
            }
        };
    }

    private async Task<IPaymentForm> HandleResaleGiftPaymentForm(
        IRequestInput input, 
        TInputInvoiceStarGiftResale resaleInvoice)
    {
        logger.LogInformation("GetPaymentForm for resale gift - Slug={Slug}", resaleInvoice.Slug);

        // Ищем подарок по slug прямо в базе, а не разбираем slug вручную
        var collection = mongoDatabase.GetCollection<StarGiftReadModel>("eventflow-stargiftreadmodel");
        var filter = Builders<StarGiftReadModel>.Filter.Eq(x => x.UniqueSlug, resaleInvoice.Slug) &
                     Builders<StarGiftReadModel>.Filter.Eq(x => x.ForResale, true);
        
        var giftReadModel = await collection.Find(filter).FirstOrDefaultAsync();
        
        if (giftReadModel == null)
        {
            logger.LogWarning("Resale gift not found for slug: {Slug}", resaleInvoice.Slug);
            throw new RpcException(new RpcError(400, "GIFT_NOT_FOUND"));
        }
        
        var giftId = giftReadModel.GiftId;
        var collectibleId = giftReadModel.CollectibleId ?? 0;
        var resaleStars = giftReadModel.ResaleStars ?? 0;
        var sellerId = giftReadModel.ToUserId;
        var aggregateId = giftReadModel.AggregateId ?? giftReadModel.Id;
        var title = giftReadModel.Title ?? $"Gift #{giftId}";

        logger.LogInformation("Found resale gift: GiftId={GiftId}, CollectibleId={CollectibleId}, Stars={Stars}, Seller={Seller}",
            giftId, collectibleId, resaleStars, sellerId);

        // Генерируем идентификатор формы
        var formId = (long)(uint)Guid.NewGuid().GetHashCode();

        // Достаём получателя из ToId
        var recipientPeerId = resaleInvoice.ToId switch
        {
            TInputPeerUser peerUser => peerUser.UserId,
            TInputPeerChat peerChat => peerChat.ChatId,
            TInputPeerChannel peerChannel => peerChannel.ChannelId,
            _ => throw new RpcException(new RpcError(400, "PEER_ID_INVALID"))
        };

        // Кешируем контекст платежа вместе с данными о перепродаже
        var cacheItem = new PaymentFormCacheItem
        {
            GiftId = giftId,
            ToUserId = sellerId, // звёзды получит продавец
            ToPeerId = recipientPeerId, // получатель подарка после покупки
            IncludeUpgrade = false,
            TotalStars = resaleStars,
            Message = null,
            HideName = false,
            IsResale = true, // помечаем как перепродажу
            ResaleSlug = resaleInvoice.Slug,
            AggregateId = aggregateId
        };

        await cacheManager.SetAsync(formId.ToString(), cacheItem);

        logger.LogInformation("Resale payment form created: FormId={FormId}, Stars={Stars}, Seller={SellerId}",
            formId, resaleStars, sellerId);

        // Возвращаем форму оплаты для подарка с перепродажи (используем TPaymentFormStarGift, а не TPaymentFormStars)
        return new TPaymentFormStarGift
        {
            FormId = formId,
            Invoice = new TInvoice
            {
                Test = false,
                NameRequested = false,
                PhoneRequested = false,
                EmailRequested = false,
                ShippingAddressRequested = false,
                Flexible = false,
                PhoneToProvider = false,
                EmailToProvider = false,
                Recurring = false,
                Currency = "XTR",
                Prices = new TVector<ILabeledPrice>
                {
                    new TLabeledPrice
                    {
                        Label = $"Buy Gift: {title}",
                        Amount = resaleStars
                    }
                }
            }
        };
    }

    private async Task<IPaymentForm> HandleTransferGiftPaymentForm(
        IRequestInput input, 
        TInputInvoiceStarGiftTransfer transferInvoice)
    {
        logger.LogInformation("GetPaymentForm for gift transfer - ToId={ToId}", transferInvoice.ToId);

        // Находим подарок по переданным данным сохранённого подарка
        string giftInstanceId;
        if (transferInvoice.Stargift is TInputSavedStarGiftUser userGift)
        {
            var giftReadModel = await queryProcessor.ProcessAsync(
                new GetStarGiftByMessageIdQuery(input.UserId, userGift.MsgId));
            
            if (giftReadModel == null)
            {
                logger.LogWarning("Gift not found: UserId={UserId}, MsgId={MsgId}", input.UserId, userGift.MsgId);
                throw new RpcException(new RpcError(400, "GIFT_NOT_FOUND"));
            }

            giftInstanceId = giftReadModel.AggregateId ?? giftReadModel.Id;
        }
        else if (transferInvoice.Stargift is TInputSavedStarGiftChat chatGift)
        {
            var peerId = chatGift.Peer switch
            {
                TInputPeerUser user => user.UserId,
                TInputPeerChat chat => chat.ChatId,
                TInputPeerChannel channel => channel.ChannelId,
                _ => throw new RpcException(new RpcError(400, "PEER_ID_INVALID"))
            };
            var giftReadModel = await queryProcessor.ProcessAsync(
                new GetStarGiftByMessageIdQuery(input.UserId, 0, peerId: peerId)); // SavedId — не messageId, но в чате подарок нужно искать именно по SavedId

            // GetStarGiftByMessageIdQuery ищет по MessageId, OutboxMessageId или SavedId (приведённому к long).
            // chatGift.SavedId как раз содержит SavedId.
            // В GetStarGiftByMessageIdQueryHandler условие: (x.ToUserId == query.UserId && x.SavedId == (long)query.MessageId),
            // поэтому SavedId можно передать как messageId.
             giftReadModel = await queryProcessor.ProcessAsync(
                new GetStarGiftByMessageIdQuery(
                    input.UserId,
                    (int)chatGift.SavedId, // приводим SavedId к параметру messageId согласно логике обработчика
                    peerId: peerId
                ));

            if (giftReadModel == null)
            {
                // Запасной вариант: ищем напрямую в базе, если запрос ничего не вернул
                var collection = mongoDatabase.GetCollection<BsonDocument>("eventflow-stargiftreadmodel");
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("ToPeerId", peerId),
                    Builders<BsonDocument>.Filter.Eq("SavedId", chatGift.SavedId)
                );
                var giftDoc = await collection.Find(filter).FirstOrDefaultAsync();

                if (giftDoc == null)
                {
                    logger.LogWarning("Gift not found in chat: UserId={UserId}, PeerId={PeerId}, SavedId={SavedId}", input.UserId, peerId, chatGift.SavedId);
                    throw new RpcException(new RpcError(400, "GIFT_NOT_FOUND"));
                }
                
                var aggregateIdValue = giftDoc.GetValue("AggregateId", BsonNull.Value);
                giftInstanceId = aggregateIdValue.IsBsonNull ? giftDoc.GetValue("_id").AsString : aggregateIdValue.AsString;
            }
            else 
            {
                giftInstanceId = giftReadModel.AggregateId ?? giftReadModel.Id;
            }
        }


        else
        {
            throw new RpcException(new RpcError(400, "INVALID_GIFT_INPUT"));
        }

        // Определяем получателя
        long recipientUserId = 0;
        if (transferInvoice.ToId is TInputPeerUser peerUser)
        {
            recipientUserId = peerUser.UserId;
        }
        else
        {
            throw new RpcException(new RpcError(400, "PEER_ID_INVALID"));
        }

        // Генерируем идентификатор формы
        var formId = (long)(uint)Guid.NewGuid().GetHashCode();

        // Кешируем контекст платежа для передачи подарка
        var cacheItem = new PaymentFormCacheItem
        {
            GiftId = 0, // для передачи не нужен
            ToUserId = recipientUserId,
            ToPeerId = recipientUserId,
            IncludeUpgrade = false,
            TotalStars = 0, // у передачи может быть стоимость (поле TransferStars)
            Message = null,
            HideName = false,
            IsResale = false,
            IsTransfer = true,
            AggregateId = giftInstanceId
        };

        await cacheManager.SetAsync(formId.ToString(), cacheItem);

        logger.LogInformation("Transfer payment form created: FormId={FormId}, RecipientUserId={RecipientUserId}",
            formId, recipientUserId);

        // Получаем данные получателя
        var users = new TVector<IUser>();
        var recipientUser = await queryProcessor.ProcessAsync(
            new GetUserByIdQuery(recipientUserId),
            CancellationToken.None);
        
        if (recipientUser != null)
        {
            var layeredUser = userConverterService.ToUser(input, recipientUser);

            // Принудительно скрываем телефон получателя при передаче подарка
            if (layeredUser is TUser user)
            {
                user.Phone = null;
                user.ComputeFlag();
                logger.LogInformation("[PRIVACY] Forced hiding phone for transfer recipient {UserId}", recipientUser.UserId);
            }

            users.Add(layeredUser);
        }

        // Возвращаем форму оплаты для передачи (обычно бесплатно или со стоимостью TransferStars)
        return new TPaymentFormStars
        {
            FormId = formId,
            BotId = 0,
            Title = "Transfer Gift",
            Description = "Transfer gift to another user",
            Photo = null,
            Invoice = new TInvoice
            {
                Test = false,
                NameRequested = false,
                PhoneRequested = false,
                EmailRequested = false,
                ShippingAddressRequested = false,
                Flexible = false,
                PhoneToProvider = false,
                EmailToProvider = false,
                Recurring = false,
                Currency = "XTR",
                Prices = new TVector<ILabeledPrice>
                {
                    new TLabeledPrice
                    {
                        Label = "Transfer",
                        Amount = 0
                    }
                }
            },
            Users = [.. users, userConverterService.ToUser(input, await queryProcessor.ProcessAsync(new GetUserByIdQuery(input.UserId), CancellationToken.None) ?? throw new RpcException(new RpcError(401, "AUTH_KEY_INVALID")))]
        };
    }
}
