namespace MyTelegram.Messenger.Services.Caching;

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.QueryHandlers.MongoDB;

internal sealed class AccessHashHelper(
    IQueryProcessor queryProcessor,
    IReadModelCacheHelper<IUserReadModel> useReadModelCacheHelper,
    IAccessHashHelper2 accessHashHelper2,
    IPeerHelper peerHelper)
    : IAccessHashHelper, ISingletonDependency
{
    private readonly ConcurrentDictionary<long, long> _accessHashCaches = new();

    public void AddAccessHash(long id, long accessHash)
    {
        _accessHashCaches.TryAdd(id, accessHash);
    }


    public async Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, long id,
        long accessHash, AccessHashType? accessHashType = null)
    {
        if (!await IsAccessHashValidAsync(request, id, accessHash, accessHashType))
        {
            switch (accessHashType)
            {
                case AccessHashType.WallPaper:
                    RpcErrors.RpcErrors400.WallpaperInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Theme:
                    RpcErrors.RpcErrors400.ThemeInvalid.ThrowRpcError();
                    break;
                case AccessHashType.GroupCall:
                    RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
                    break;
                case AccessHashType.StickerSet:
                    RpcErrors.RpcErrors400.StickersetInvalid.ThrowRpcError();
                    break;
                case AccessHashType.User:
                    RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Channel:
                    RpcErrors.RpcErrors400.ChannelIdInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Document:
                    RpcErrors.RpcErrors400.DocumentInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Photo:
                    RpcErrors.RpcErrors400.PhotoInvalid.ThrowRpcError();
                    break;
                case AccessHashType.Sticker:
                    RpcErrors.RpcErrors400.StickersetInvalid.ThrowRpcError();
                    break;
                default:
                    RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
                    break;
            }
        }
    }

    public Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, IInputPeer? inputPeer) =>
        inputPeer switch
        {
            TInputPeerChannel inputPeerChannel => CheckAccessHashAsync(request, inputPeerChannel.ChannelId,
                inputPeerChannel.AccessHash, AccessHashType.Channel),
            TInputPeerChannelFromMessage inputPeerChannelFromMessage => CheckAccessHashAsync(request, inputPeerChannelFromMessage
                .Peer),
            TInputPeerUser inputPeerUser => CheckAccessHashAsync(request, inputPeerUser.UserId, inputPeerUser.AccessHash, AccessHashType.User),
            TInputPeerUserFromMessage inputPeerUserFromMessage => CheckAccessHashAsync(request, inputPeerUserFromMessage.Peer),
            _ => Task.CompletedTask
        };

    public Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, IInputUser inputUser)
    {
        if (inputUser is TInputUser tInputUser)
        {
            return CheckAccessHashAsync(request, tInputUser.UserId, tInputUser.AccessHash);
        }

        return Task.CompletedTask;
    }

    public Task CheckAccessHashAsync(IRequestWithAccessHashKeyId request, IInputChannel inputChannel)
    {
        if (inputChannel is TInputChannel tInputChannel)
        {
            return CheckAccessHashAsync(request, tInputChannel.ChannelId, tInputChannel.AccessHash, AccessHashType.Channel);
        }

        return Task.CompletedTask;
    }

    public async Task<bool> IsAccessHashValidAsync(IRequestWithAccessHashKeyId request, long id,
                        long accessHash, AccessHashType? accessHashType = null)
    {
        if (accessHashType == null)
        {
            var peer = peerHelper.GetPeer(id);
            switch (peer.PeerType)
            {
                case PeerType.Channel:
                    accessHashType = AccessHashType.Channel;
                    break;
                case PeerType.User:
                    accessHashType = AccessHashType.User;
                    break;
                case PeerType.Self:
                    return true;
            }
        }

        switch (accessHashType)
        {
            case AccessHashType.User:
            case AccessHashType.Channel:
                // Try new session-based access hash first
                var isValidForCurrentSession = await accessHashHelper2.IsAccessHashValidAsync(request, id, accessHash, accessHashType);
                if (isValidForCurrentSession)
                {
                    return true;
                }
                // Fallback: check against stored access hash (for backward compatibility and different sessions)
                break;
        }

        if (_accessHashCaches.TryGetValue(id, out var cachedAccessHash))
        {
            return accessHash == cachedAccessHash;
        }

        var accessHashReadModel = await queryProcessor.ProcessAsync(new GetAccessHashQueryByIdQuery(id));

        if (accessHashReadModel != null)
        {
            _accessHashCaches.TryAdd(accessHashReadModel.AccessId, accessHashReadModel.AccessHash);
            return accessHash == accessHashReadModel.AccessHash;
        }

        switch (accessHashType)
        {
            case AccessHashType.User:
                var userReadModel = await useReadModelCacheHelper.GetOrCreateAsync(id,
                    () => queryProcessor.ProcessAsync(new GetUserByIdQuery(id)), p => p.Id);
                if (userReadModel != null)
                {
                    _accessHashCaches.TryAdd(id, userReadModel.AccessHash);
                    return accessHash == userReadModel.AccessHash;
                }

                break;

            case AccessHashType.Channel:
                var channelReadModel = await queryProcessor.ProcessAsync(new GetChannelByIdQuery(id));
                if (channelReadModel != null)
                {
                    _accessHashCaches.TryAdd(id, channelReadModel.AccessHash);
                    return accessHash == channelReadModel.AccessHash;
                }

                break;

            case AccessHashType.WallPaper:
                var wallPaperReadModel = await queryProcessor.ProcessAsync(new GetWallPaperQuery(id));
                if (wallPaperReadModel != null)
                {
                    _accessHashCaches.TryAdd(id, wallPaperReadModel.AccessHash);
                    return accessHash == wallPaperReadModel.AccessHash;
                }
                break;
            case AccessHashType.Theme:
                var themeReadModel = await queryProcessor.ProcessAsync(new GetThemeByIdQuery(id));
                if (themeReadModel != null)
                {
                    _accessHashCaches.TryAdd(id, themeReadModel.Theme.AccessHash);

                    return accessHash == themeReadModel.Theme.AccessHash;
                }
                break;
            case AccessHashType.GroupCall:
                var groupCallReadModel = await queryProcessor.ProcessAsync(
                    new GetGroupCallByIdQuery(GroupCallId.Create(id).Value));
                if (groupCallReadModel != null)
                {
                    _accessHashCaches.TryAdd(groupCallReadModel.CallId, groupCallReadModel.AccessHash);
                    return accessHash == groupCallReadModel.AccessHash;
                }
                break;
            case AccessHashType.StickerSet:
                var stickerSetReadModel = await queryProcessor.ProcessAsync(new GetStickerSetByIdQuery(id));
                if (stickerSetReadModel != null)
                {
                    _accessHashCaches.TryAdd(stickerSetReadModel.StickerSetId, stickerSetReadModel.AccessHash);

                    return accessHash == stickerSetReadModel.AccessHash;
                }
                break;
            case AccessHashType.Document:
                var documentReadModel = await queryProcessor.ProcessAsync(new GetDocumentByIdQuery(id));
                if (documentReadModel != null)
                {
                    _accessHashCaches.TryAdd(documentReadModel.DocumentId, documentReadModel.AccessHash);
                    return accessHash == documentReadModel.AccessHash;
                }
                break;
            case AccessHashType.Photo:
                var photoReadModel = await queryProcessor.ProcessAsync(new GetPhotoByIdQuery(id));
                if (photoReadModel != null)
                {
                    _accessHashCaches.TryAdd(photoReadModel.PhotoId, photoReadModel.AccessHash);
                    return accessHash == photoReadModel.AccessHash;
                }
                break;
            //case AccessHashType.Sticker:
            //    break;
            default:
                throw new ArgumentOutOfRangeException(nameof(accessHashType), accessHashType, null);
        }

        return false;
    }
}

