using Google.Protobuf;
using MyTelegram.Domain.Aggregates.Photo;
using MyTelegram.Domain.Commands.Photo;
using TWebPage = MyTelegram.Schema.TWebPage;

namespace MyTelegram.Messenger.Services.Impl;

public class MediaHelper(
    IOptionsMonitor<MyTelegramMessengerServerOptions> options,
    ICacheManager<UserCacheItem> cacheManager,
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper,
    IObjectMapper objectMapper,
    ICommandBus commandBus,
    IFilePartManager filePartManager,
    ILogger<MediaHelper> logger)
    : IMediaHelper, ITransientDependency
{
    public MessageType GeMessageType(IMessageMedia? media)
    {
        if (media == null)
        {
            return MessageType.Text;
        }

        return media switch
        {
            TMessageMediaContact => MessageType.Contacts,
            TMessageMediaDice => MessageType.Game,
            TMessageMediaDocument => MessageType.Document,
            TMessageMediaEmpty => MessageType.Text,
            TMessageMediaGame => MessageType.Game,
            TMessageMediaGeo => MessageType.Geo,
            TMessageMediaGeoLive => MessageType.Geo,
            TMessageMediaInvoice => MessageType.Voice,
            TMessageMediaPhoto => MessageType.Photo,
            TMessageMediaPoll => MessageType.Poll,
            TMessageMediaUnsupported => MessageType.Text,
            TMessageMediaVenue => MessageType.Geo,
            TMessageMediaWebPage => MessageType.Url,
            _ => throw new ArgumentOutOfRangeException(nameof(media))
        };
    }

    public async Task<IEncryptedFile> SaveEncryptedFileAsync(long reqMsgId,
            IInputEncryptedFile encryptedFile)
    {
        var client = GrpcClientFactory.CreateMediaServiceClient(options.CurrentValue.FileServerGrpcServiceUrl);
        var r = await client
            .SaveEncryptedFileAsync(new SaveEncryptedFileRequest
            {
                EncryptedFile = ByteString.CopyFrom(encryptedFile.ToBytes()),
                ReqMsgId = reqMsgId
            }).ResponseAsync;

        return new TEncryptedFile
        {
            AccessHash = r.AccessHash,
            DcId = r.DcId,
            Id = r.Id,
            KeyFingerprint = r.KeyFingerprint,
            Size = r.Size
        };
    }

    public Task<IMessageMedia?> SaveMediaAsync(IInputMedia? media, long userId = 0)
    {
        return SaveMediaCoreAsync(media, userId);
    }

    private async Task CreatePhotoAsync(long userId, IPhoto photo, bool hasVideo, int size, bool isProfilePhoto)
    {

        switch (photo)
        {
            case TPhoto photo1:
                var command = new CreatePhotoCommand(PhotoId.Create(photo1.Id),
                    userId,
                    new PhotoItem(photo1.Id, photo1.AccessHash, photo1.FileReference, photo1.Date,
                        photo1.DcId,
                        size,
                        false,
                        hasVideo,
                        IsProfilePhoto: isProfilePhoto,
                        Sizes2: [.. photo1.Sizes],
                        VideoSizes2: photo1.VideoSizes?.ToList()
                    )
                );
                await commandBus.PublishAsync(command);
                break;
        }
    }

    private List<PhotoSize>? ToPhotoSize(TVector<IPhotoSize> photoSizes)
    {
        List<PhotoSize>? sizes = null;
        foreach (var photoSize in photoSizes)
        {
            switch (photoSize)
            {
                case TPhotoSize photoSize1:
                    sizes ??= [];
                    sizes.Add(new PhotoSize(photoSize1.W, photoSize1.H, photoSize1.Size, photoSize1.Type));
                    break;
            }
        }

        return sizes;
    }

    private List<VideoSize>? ToVideoSizes(TVector<IVideoSize>? videoSizes)
    {
        List<VideoSize>? sizes = null;
        foreach (var videoSize in videoSizes ?? [])
        {
            switch (videoSize)
            {
                case TVideoSize videoSize1:
                    sizes ??= [];
                    sizes.Add(new VideoSize(videoSize1.W, videoSize1.H, videoSize1.Size, videoSize1.Type, videoSize1.VideoStartTs ?? 0));
                    break;
            }
        }

        return sizes;
    }

    public async Task<SavePhotoResult> SavePhotoAsync(long reqMsgId,
            long userId,
        long fileId,
        bool hasVideo,
        double? videoStartTs,
        int parts,
        string? name,
        string? md5,
        IVideoSize? videoEmojiMarkup = null,
        bool isProfilePhoto = false
        )
    {
        var client = GrpcClientFactory.CreateMediaServiceClient(options.CurrentValue.FileServerGrpcServiceUrl);

        var r = await client.SavePhotoAsync(new SavePhotoRequest
        {
            UserId = userId,
            FileId = fileId,
            HasVideo = hasVideo,
            Md5 = md5 ?? string.Empty,
            Name = name ?? string.Empty,
            Parts = parts,
            ReqMsgId = reqMsgId,
            VideoStartTs = videoStartTs ?? 0,
            VideoEmojiMarkup = videoEmojiMarkup == null ? ByteString.Empty : ByteString.CopyFrom(videoEmojiMarkup.ToBytes()),
            IsProfilePhoto = isProfilePhoto

        }).ResponseAsync;

        var photo = r.Photo.Memory.ToTObject<IPhoto>();
        await CreatePhotoAsync(userId, photo, hasVideo, (int)r.Size, isProfilePhoto);

        return new SavePhotoResult(r.PhotoId, r.Photo.Memory.ToTObject<IPhoto>());
    }

    private async Task<TMessageMediaContact> CreateMediaContactAsync(TInputMediaContact inputMediaContact)
    {
        var cachedUserItem = await cacheManager
                .GetAsync(UserCacheItem.GetCacheKey(inputMediaContact.PhoneNumber))
            ;
        return new TMessageMediaContact
        {
            FirstName = inputMediaContact.FirstName,
            LastName = inputMediaContact.LastName ?? string.Empty,
            PhoneNumber = inputMediaContact.PhoneNumber?.Replace(" ", string.Empty) ?? string.Empty,
            Vcard = inputMediaContact.Vcard ?? string.Empty,
            UserId = cachedUserItem?.UserId ?? 0
        };
    }

    private IMessageMedia CreateMediaDice(TInputMediaDice inputMediaDice)
    {
        int value;// значение кубика: 1-6
        switch (inputMediaDice.Emoticon)
        {
            case "🎰": // Слот-машина, значение: 0-65
                value = Random.Shared.Next(0, 65);
                break;
            case "⚽️":
            case "🏀":
                value = Random.Shared.Next(1, 6);
                break;
            default:// значение кубика: 1-6
                value = Random.Shared.Next(1, 7);
                break;
        }

        return new TMessageMediaDice
        {
            Emoticon = inputMediaDice.Emoticon,
            Value = value
        };
    }

    private IMessageMedia CreateMediaGeoLive(TInputMediaGeoLive inputMediaGeoLive)
    {
        IGeoPoint geo = new TGeoPointEmpty();
        if (inputMediaGeoLive.GeoPoint is TInputGeoPoint inputGeoPoint1)
        {
            geo = new TGeoPoint
            {
                AccuracyRadius = inputGeoPoint1.AccuracyRadius,
                Lat = inputGeoPoint1.Lat,
                Long = inputGeoPoint1.Long,
                AccessHash = Random.Shared.NextInt64()
            };
        }

        return new TMessageMediaGeoLive
        {
            Heading = inputMediaGeoLive.Heading,
            Period = inputMediaGeoLive.Period ?? 0,
            ProximityNotificationRadius = inputMediaGeoLive.ProximityNotificationRadius,
            Geo = geo
        };
    }

    private IMessageMedia CreateMediaGeoPoint(TInputMediaGeoPoint inputMediaGeoPoint)
    {
        switch (inputMediaGeoPoint.GeoPoint)
        {
            case TInputGeoPoint inputGeoPoint1:
                return new TMessageMediaGeo
                {
                    Geo = new TGeoPoint
                    {
                        AccuracyRadius = inputGeoPoint1.AccuracyRadius,
                        Lat = inputGeoPoint1.Lat,
                        Long = inputGeoPoint1.Long,
                        AccessHash = Random.Shared.NextInt64()
                    }
                };
        }

        return new TMessageMediaGeo
        {
            Geo = new TGeoPointEmpty()
        };
    }

    private async Task<IMessageMedia> CreateMediaOnFileServerAsync(IInputMedia media, long userId = 0)
    {
        try
        {
            logger.LogWarning("*** CreateMediaOnFileServerAsync called with media type: {MediaType}", media?.GetType().Name);

            // Отдельная обработка InputMediaUploadedDocument (новые загрузки)
            if (media is TInputMediaUploadedDocument uploadedDoc)
            {
                logger.LogWarning("*** Handling InputMediaUploadedDocument (new upload)");
                return await CreateMediaFromUploadedDocumentAsync(uploadedDoc, userId);
            }

            // Отдельная обработка InputMediaDocument (существующие стикеры и документы)
            if (media is TInputMediaDocument inputMediaDocument)
            {
                logger.LogWarning("*** Handling InputMediaDocument (existing sticker) - DocumentId: {DocumentId}",
                    (inputMediaDocument.Id as TInputDocument)?.Id ?? 0);
                return await CreateMediaFromExistingDocumentAsync(inputMediaDocument);
            }

            logger.LogWarning("*** Falling back to file-server for media type: {MediaType}", media?.GetType().Name);
            var client = GrpcClientFactory.CreateMediaServiceClient(options.CurrentValue.FileServerGrpcServiceUrl);
            var r = await client.SaveMediaAsync(new SaveMediaRequest
            {
                Media = ByteString.CopyFrom(media.ToBytes()),
                UserId = userId
            })
                .ResponseAsync;
            return r.Media.Memory.ToTObject<IMessageMedia>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Save media failed");
            RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
        }

        throw new InvalidOperationException();
    }

    private async Task<IMessageMedia> CreateMediaFromExistingDocumentAsync(TInputMediaDocument inputMediaDocument)
    {
        try
        {
            // Получаем сведения о документе из базы
            if (inputMediaDocument.Id is not TInputDocument inputDocument)
            {
                logger.LogWarning("Invalid InputDocument type: {Type}", inputMediaDocument.Id?.GetType().Name);
                RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
                throw new InvalidOperationException("Invalid document");
            }

            logger.LogInformation(
                "Processing existing document: documentId={DocumentId}, accessHash={AccessHash}",
                inputDocument.Id, inputDocument.AccessHash);

            // Загружаем документ из базы
            var document = await queryProcessor.ProcessAsync(new GetDocumentByIdQuery(inputDocument.Id), default);
            
            if (document == null)
            {
                logger.LogWarning("Document not found: {DocumentId}", inputDocument.Id);
                RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
                throw new InvalidOperationException("Document not found");
            }

            // Формируем TDocument на основе DocumentReadModel
            var tDocument = new TDocument
            {
                Id = document.DocumentId,
                AccessHash = document.AccessHash,
                FileReference = document.FileReference.IsEmpty ? Array.Empty<byte>() : document.FileReference.ToArray(),
                Date = document.Date,
                MimeType = document.MimeType ?? "application/octet-stream",
                Size = document.Size,
                DcId = document.DcId,
                Attributes = document.Attributes2 != null 
                    ? new TVector<IDocumentAttribute>(document.Attributes2) 
                    : new TVector<IDocumentAttribute>()
            };

            // Добавляем thumbnail-ы, если они есть
            if (document.Thumbs != null && document.Thumbs.Count > 0)
            {
                // Пока пропускаем преобразование thumbs, чтобы избежать проблем с приведением типов
                // TODO: корректно реализовать преобразование PhotoSize в IPhotoSize
                // tDocument.Thumbs = new TVector<IPhotoSize>(document.Thumbs.Cast<IPhotoSize>());
            }

            // Премиальные стикеры: копируем VideoThumbs (эффекты)
            if (document.VideoThumbs != null && document.VideoThumbs.Count > 0)
            {
                var videoThumbs = document.VideoThumbs.Select(vt => new TVideoSize
                {
                    Type = vt.Type,
                    W = vt.W,
                    H = vt.H,
                    Size = (int)vt.Size,  // Приводим long к int
                    VideoStartTs = vt.VideoStartTs
                }).Cast<IVideoSize>();
                tDocument.VideoThumbs = new TVector<IVideoSize>(videoThumbs);

                logger.LogInformation("Premium sticker: DocumentId={DocumentId}, VideoThumbs count={Count}",
                    document.DocumentId, document.VideoThumbs.Count);
            }

            logger.LogInformation("Document loaded successfully: {DocumentId}, MimeType={MimeType}, Size={Size}",
                document.DocumentId, document.MimeType, document.Size);

            // Возвращаем MessageMediaDocument
            return new TMessageMediaDocument
            {
                Document = tDocument,
                TtlSeconds = inputMediaDocument.TtlSeconds
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create media from existing document");
            RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
            throw;
        }
    }

    private async Task<IMessageMedia> CreateMediaFromUploadedDocumentAsync(TInputMediaUploadedDocument uploadedDoc, long userId)
    {
        try
        {
            // Извлекаем сведения о файле из InputFile
            if (uploadedDoc.File is not TInputFile inputFile)
            {
                RpcErrors.RpcErrors400.FileIdInvalid.ThrowRpcError();
                throw new InvalidOperationException("Invalid file");
            }

            logger.LogInformation(
                "Processing uploaded document: fileId={FileId}, parts={Parts}, name={Name}, mimeType={MimeType}",
                inputFile.Id, inputFile.Parts, inputFile.Name, uploadedDoc.MimeType);

            // Склеиваем части файла
            var (mergedFilePath, totalSize) = await filePartManager.MergeFilePartsAsync(
                userId, inputFile.Id, inputFile.Parts, inputFile.Name);

            try
            {
                // Читаем собранный файл
                var fileData = await File.ReadAllBytesAsync(mergedFilePath);

                // Отправляем на file-server для обработки
                var client = GrpcClientFactory.CreateMediaServiceClient(options.CurrentValue.FileServerGrpcServiceUrl);
                var response = await client.SaveMediaAsync(new SaveMediaRequest
                {
                    Media = ByteString.CopyFrom(uploadedDoc.ToBytes()),
                    UserId = userId
                }).ResponseAsync;

                // Очистка
                await filePartManager.CleanupFilePartsAsync(userId, inputFile.Id);

                return response.Media.Memory.ToTObject<IMessageMedia>();
            }
            finally
            {
                // Всегда удаляем временные файлы
                try
                {
                    if (File.Exists(mergedFilePath))
                    {
                        File.Delete(mergedFilePath);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to delete merged file: {FilePath}", mergedFilePath);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create media from uploaded document");
            throw;
        }
    }

    private IMessageMedia CreateMediaPoll(TInputMediaPoll inputMediaPoll)
    {
        return new TMessageMediaPoll
        {
            Poll = inputMediaPoll.Poll,
            Results = new TPollResults()
        };
    }

    private Task<IMessageMedia> CreateMediaStoryAsync(TInputMediaStory inputMediaStory)
    {
        throw new NotImplementedException();
    }

    private IMessageMedia CreateMediaVenue(TInputMediaVenue inputMediaVenue)
    {
        TInputGeoPoint? inputGeoPoint = null;
        if (inputMediaVenue.GeoPoint is TInputGeoPoint geoPoint)
        {
            inputGeoPoint = geoPoint;
        }

        return new TMessageMediaVenue
        {
            Title = inputMediaVenue.Title,
            Address = inputMediaVenue.Address,
            Provider = inputMediaVenue.Provider,
            VenueId = inputMediaVenue.VenueId,
            VenueType = inputMediaVenue.VenueType,
            Geo = inputGeoPoint == null
                ? new TGeoPointEmpty()
                : new TGeoPoint
                {
                    AccuracyRadius = inputGeoPoint.AccuracyRadius,
                    Lat = inputGeoPoint.Lat,
                    Long = inputGeoPoint.Long
                }
        };
    }

    private Task<IMessageMedia> CreateMediaWebPageAsync(TInputMediaWebPage inputMediaWebPage)
    {
        throw new NotImplementedException();
    }

    private async Task<IMessageMedia?> SaveMediaCoreAsync(IInputMedia? media, long userId = 0)
    {
        switch (media)
        {
            case TInputMediaContact inputMediaContact:
                return await CreateMediaContactAsync(inputMediaContact);
            case TInputMediaDice inputMediaDice:
                return CreateMediaDice(inputMediaDice);
            case TInputMediaDocument:
            case TInputMediaDocumentExternal:
            case TInputMediaPhoto:
            case TInputMediaPhotoExternal:
            case TInputMediaUploadedDocument:
            case TInputMediaUploadedPhoto:
                return await CreateMediaOnFileServerAsync(media, userId);
            case TInputMediaPaidMedia:
            case TInputMediaGame:
            case TInputMediaInvoice:
                throw new NotImplementedException();
            case TInputMediaEmpty:
                return new TMessageMediaEmpty();
            case TInputMediaGeoLive inputMediaGeoLive:
                return CreateMediaGeoLive(inputMediaGeoLive);
            case TInputMediaGeoPoint inputMediaGeoPoint:
                return CreateMediaGeoPoint(inputMediaGeoPoint);
            case TInputMediaPoll inputMediaPoll:
                return CreateMediaPoll(inputMediaPoll);
            case TInputMediaStory inputMediaStory:
                return await CreateMediaStoryAsync(inputMediaStory);
            case TInputMediaVenue inputMediaVenue:
                return CreateMediaVenue(inputMediaVenue);
            case TInputMediaWebPage inputMediaWebPage:
                return await CreateMediaWebPageAsync(inputMediaWebPage);
            default:
                return null;
        }
    }
}
