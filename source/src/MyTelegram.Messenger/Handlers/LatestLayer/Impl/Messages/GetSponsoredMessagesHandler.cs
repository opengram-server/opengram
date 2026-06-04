// ReSharper disable All

using MyTelegram.Queries.SponsoredMessages;
using System.Security.Cryptography;
using MongoDB.Driver;
using MyTelegram.Schema.Payments;
using MyTelegram.Queries;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Возвращает список рекламных (спонсорских) сообщений для указанного peer.
/// See <a href="https://corefork.telegram.org/method/messages.getSponsoredMessages" />
///</summary>
internal sealed class GetSponsoredMessagesHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetSponsoredMessages, MyTelegram.Schema.Messages.ISponsoredMessages>,
    Messages.IGetSponsoredMessagesHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<GetSponsoredMessagesHandler> _logger;
    private readonly IMongoDatabase _database;

    public GetSponsoredMessagesHandler(IQueryProcessor queryProcessor, ILogger<GetSponsoredMessagesHandler> logger, IMongoDatabase database)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
        _database = database;
    }

    protected override async Task<MyTelegram.Schema.Messages.ISponsoredMessages> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetSponsoredMessages obj)
    {
        _logger.LogInformation("GetSponsoredMessages called - UserId={UserId}, Peer={Peer}", input.UserId, obj.Peer);

        // Достаём id канала из peer
        long channelId = 0;
        if (obj.Peer is TInputPeerChannel inputPeerChannel)
        {
            channelId = inputPeerChannel.ChannelId;
        }
        else if (obj.Peer is TInputPeerChannelFromMessage inputPeerChannelFromMessage)
        {
            channelId = inputPeerChannelFromMessage.ChannelId;
        }
        else
        {
            // Рекламные сообщения поддерживаются только для каналов
            _logger.LogWarning("Sponsored messages requested for non-channel peer: {PeerType}", obj.Peer.GetType().Name);
            return new TSponsoredMessagesEmpty();
        }

        // Загружаем рекламные сообщения для этого канала
        var sponsoredMessages = await _queryProcessor.ProcessAsync(
            new GetSponsoredMessagesByChannelQuery(channelId, onlyActive: true));

        var messages = new List<ISponsoredMessage>();
        
        if (sponsoredMessages != null && sponsoredMessages.Any())
        {
            _logger.LogInformation("Found {Count} sponsored messages for channel {ChannelId}", sponsoredMessages.Count, channelId);
            var collection = _database.GetCollection<MyTelegram.ReadModel.Impl.SponsoredMessageReadModel>("ReadModel-SponsoredMessageReadModel");
            
            foreach (var sm in sponsoredMessages)
            {
                // Увеличиваем счётчик показов рекламы
                try
                {
                    var filter = Builders<MyTelegram.ReadModel.Impl.SponsoredMessageReadModel>.Filter.Eq(x => x.Id, sm.Id);
                    var update = Builders<MyTelegram.ReadModel.Impl.SponsoredMessageReadModel>.Update.Inc(x => x.DisplayCount, 1);
                    await collection.UpdateOneAsync(filter, update);
                    
                    _logger.LogInformation("Incremented display count for ad: {Title} (ID: {Id})", sm.Title, sm.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to increment display count: {Error}", ex.Message);
                }

                // Генерируем random_id (обязательное поле по схеме)
                var randomId = GenerateRandomId();

                var message = new TSponsoredMessage
                {
                    Recommended = sm.Recommended,
                    CanReport = sm.CanReport,
                    RandomId = randomId,
                    Url = sm.Url,
                    Title = sm.Title,
                    Message = sm.Message,
                    Entities = null, // TODO: при необходимости разбирать entities из текста
                    Photo = null, // TODO: загружать фото, если задан PhotoUrl
                    Color = null, // необязательный цвет peer
                    ButtonText = sm.ButtonText,
                    SponsorInfo = sm.SponsorInfo,
                    AdditionalInfo = sm.AdditionalInfo
                };

                messages.Add(message);

                _logger.LogInformation("Added sponsored message: Title={Title}, Url={Url}", sm.Title, sm.Url);
            }
        }
        else
        {
            _logger.LogInformation("No sponsored messages found for channel {ChannelId}", channelId);
        }

        // Загружаем звёздные подарки для каталога канала
        var availableGifts = await _queryProcessor.ProcessAsync(new GetAvailableStarGiftsQuery(0));
        var giftsList = new List<IStarGift>();
        
        if (availableGifts != null)
        {
            foreach (var g in availableGifts)
            {
                TDocument sticker;
                
                if (g.Sticker.HasValue && g.Sticker.Value > 0)
                {
                    try
                    {
                        var document = await _queryProcessor.ProcessAsync(new GetDocumentByIdQuery(g.Sticker.Value));
                        
                        if (document != null)
                        {
                            var fileRef = document.FileReference.IsEmpty || document.FileReference.Length == 0
                                ? GenerateFileReference(document.DocumentId)
                                : document.FileReference.ToArray();
                            
                            sticker = new TDocument
                            {
                                Id = document.DocumentId,
                                AccessHash = document.AccessHash,
                                FileReference = fileRef,
                                Date = document.Date,
                                MimeType = document.MimeType ?? "application/x-tgsticker",
                                Size = document.Size,
                                DcId = document.DcId,
                                Attributes = document.Attributes2 != null 
                                    ? new TVector<IDocumentAttribute>(document.Attributes2) 
                                    : new TVector<IDocumentAttribute>()
                            };
                        }
                        else
                        {
                            sticker = new TDocument
                            {
                                Id = g.Sticker.Value,
                                AccessHash = 0,
                                FileReference = Array.Empty<byte>(),
                                Date = g.FirstSaleDate ?? DateTime.UtcNow.ToTimestamp(),
                                MimeType = "application/x-tgsticker",
                                Size = 1,
                                DcId = 2,
                                Attributes = new TVector<IDocumentAttribute>()
                            };
                        }
                    }
                    catch
                    {
                        sticker = CreatePlaceholderDocument();
                    }
                }
                else
                {
                    sticker = CreatePlaceholderDocument();
                }

                var firstSaleDate = g.FirstSaleDate;
                var lastSaleDate = g.LastSaleDate;
                if (g.SoldOut || firstSaleDate.HasValue || lastSaleDate.HasValue)
                {
                    var now = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    if (!firstSaleDate.HasValue) firstSaleDate = now;
                    if (!lastSaleDate.HasValue) lastSaleDate = now;
                }
                
                var availabilityRemains = g.AvailabilityRemains;
                var availabilityTotal = g.AvailabilityTotal;
                if (g.Limited || availabilityRemains.HasValue || availabilityTotal.HasValue)
                {
                    if (!availabilityRemains.HasValue) availabilityRemains = 0;
                    if (!availabilityTotal.HasValue) availabilityTotal = 0;
                }
                
                var availabilityResale = g.AvailabilityResale;
                var resellMinStars = g.ResellMinStars;
                if (availabilityResale.HasValue || resellMinStars.HasValue)
                {
                    if (!availabilityResale.HasValue) availabilityResale = 0;
                    if (!resellMinStars.HasValue) resellMinStars = 0;
                }
                
                var gift = new TStarGift
                {
                    Id = g.GiftId,
                    Limited = g.Limited,
                    SoldOut = g.SoldOut,
                    Birthday = g.Birthday,
                    // Убрано ради совместимости с клиентом 5.16.0
                    // RequirePremium = g.RequirePremium,
                    // LimitedPerUser = false,
                    Sticker = sticker,
                    Stars = g.Stars,
                    AvailabilityRemains = availabilityRemains,
                    AvailabilityTotal = availabilityTotal,
                    ConvertStars = g.ConvertStars,
                    FirstSaleDate = firstSaleDate,
                    LastSaleDate = lastSaleDate,
                    UpgradeStars = g.UpgradeStars,
                    Title = g.Title ?? "",
                    ResellMinStars = resellMinStars,
                    AvailabilityResale = availabilityResale,
                    // Убрано ради совместимости с клиентом 5.16.0
                    // PerUserTotal = null,
                    // PerUserRemains = null
                };
                
                giftsList.Add(gift);
            }
        }

        var calculatedHash = 0;
        if (giftsList.Any())
        {
            foreach (var gift in giftsList)
            {
                calculatedHash ^= gift.Id.GetHashCode();
            }
        }
        
        var starGifts = new TStarGifts
        {
            Hash = calculatedHash,
            Gifts = new TVector<IStarGift>(giftsList),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };

        // Значение posts_between - сколько обычных постов идёт между рекламными
        var postsBetween = sponsoredMessages?.FirstOrDefault()?.PostsBetween ?? 10;

        var result = new TSponsoredMessages
        {
            PostsBetween = postsBetween,
            Messages = new TVector<ISponsoredMessage>(messages),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };

        _logger.LogInformation("Returning {Count} sponsored messages and {GiftCount} gifts to client",
            messages.Count, giftsList.Count);

        return result;
    }

    private static byte[] GenerateRandomId()
    {
        // Генерируем 16 случайных байт для random_id
        var randomId = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomId);
        }
        return randomId;
    }

    private static TDocument CreatePlaceholderDocument()
    {
        return new TDocument
        {
            Id = 0,
            AccessHash = 0,
            FileReference = Array.Empty<byte>(),
            Date = 0,
            MimeType = "application/x-tgsticker",
            Size = 0,
            DcId = 0,
            Attributes = new TVector<IDocumentAttribute>()
        };
    }
    
    private static byte[] GenerateFileReference(long documentId)
    {
        var buffer = new byte[8];
        var timestamp = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
        buffer[0] = (byte)(timestamp >> 24);
        buffer[1] = (byte)(timestamp >> 16);
        buffer[2] = (byte)(timestamp >> 8);
        buffer[3] = (byte)timestamp;
        
        buffer[4] = (byte)(documentId >> 24);
        buffer[5] = (byte)(documentId >> 16);
        buffer[6] = (byte)(documentId >> 8);
        buffer[7] = (byte)documentId;
        
        return buffer;
    }
}
