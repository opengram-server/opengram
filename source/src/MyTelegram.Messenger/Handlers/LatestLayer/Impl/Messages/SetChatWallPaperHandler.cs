// ReSharper disable All

using MongoDB.Driver;
using MyTelegram.ReadModel;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Set a custom <a href="https://corefork.telegram.org/api/wallpapers">wallpaper »</a> in a specific private chat with another user.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// 400 WALLPAPER_INVALID The specified wallpaper is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.setChatWallPaper" />
///</summary>
internal sealed class SetChatWallPaperHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper,
    IAccessHashHelper accessHashHelper,
    IObjectMapper objectMapper,
    IIdGenerator idGenerator,
    IRandomHelper randomHelper,
    IObjectMessageSender messageSender,
    IMongoDatabase mongoDatabase)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSetChatWallPaper, MyTelegram.Schema.IUpdates>,
        Messages.ISetChatWallPaperHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSetChatWallPaper obj)
    {
        // Resolve peer
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        if (peer == null)
        {
            RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
        }

        // Only support private chats
        if (peer.PeerType != PeerType.User)
        {
            RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
        }

        var otherUserId = peer.PeerId;

        // Handle revert flag
        if (obj.Revert)
        {
            // User wants to revert to previous wallpaper
            return await RevertWallpaperAsync(input, otherUserId);
        }

        long? wallpaperId = null;
        WallPaperSettings? settings = null;

        // Get wallpaper info
        if (obj.Wallpaper != null)
        {
            wallpaperId = GetWallpaperId(obj.Wallpaper);
            if (!wallpaperId.HasValue)
            {
                RpcErrors.RpcErrors400.WallpaperInvalid.ThrowRpcError();
            }

            // Verify wallpaper exists
            var query = new GetWallpaperByIdQuery(wallpaperId.Value);
            var wallpaper = await queryProcessor.ProcessAsync(query, CancellationToken.None);

            if (wallpaper == null || wallpaper.IsDeleted)
            {
                RpcErrors.RpcErrors400.WallpaperInvalid.ThrowRpcError();
            }
        }
        else if (obj.Id.HasValue)
        {
            // Wallpaper from messageActionSetChatWallPaper
            var messageQuery = new GetMessageByIdQuery(obj.Id.Value.ToString());
            var message = await queryProcessor.ProcessAsync(messageQuery, CancellationToken.None);
            
            if (message != null)
            {
                // Extract wallpaper info from message action if available
                // For now, just skip this logic
            }
        }

        if (obj.Settings != null)
        {
            settings = ToWallPaperSettings(obj.Settings);
        }

        // Create service message
        var messageId = await idGenerator.NextIdAsync(IdType.MessageId, input.UserId, 1, CancellationToken.None);
        var date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var messageAction = new TMessageActionSetChatWallPaper
        {
            ForBoth = obj.ForBoth,
            Same = obj.Id.HasValue,
            Wallpaper = wallpaperId.HasValue 
                ? await GetWallpaperSchemaAsync(wallpaperId.Value, settings)
                : new TWallPaperNoFile { Id = 0 }
        };

        // Send updates to current user
        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = (int)date,
            Seq = 0
        };

        // Create updatePeerWallpaper
        var updatePeerWallpaper = new TUpdatePeerWallpaper
        {
            Peer = new TPeerUser { UserId = otherUserId },
            Wallpaper = messageAction.Wallpaper,
            WallpaperOverridden = obj.ForBoth
        };

        updates.Updates.Add(updatePeerWallpaper);

        // Save chat wallpaper to database
        if (wallpaperId.HasValue)
        {
            var collection = mongoDatabase.GetCollection<ChatWallPaper>("chatWallPaper");
            var chatWallPaper = new ChatWallPaper
            {
                Id = $"{input.UserId}_{otherUserId}",
                UserId = input.UserId,
                ChatId = otherUserId,
                WallPaperId = wallpaperId.Value,
                ForBoth = obj.ForBoth,
                WallPaperSettings = settings
            };
            
            await collection.ReplaceOneAsync(
                x => x.Id == chatWallPaper.Id,
                chatWallPaper,
                new ReplaceOptions { IsUpsert = true }
            );
        }

        // If for_both and user is Premium, send to other user too
        if (obj.ForBoth)
        {
            // Check if user is Premium
            var userQuery = new GetUserByIdQuery(input.UserId);
            var user = await queryProcessor.ProcessAsync(userQuery, CancellationToken.None);

            if (user?.Premium == true)
            {
                // Send update to other user
                var otherUserUpdate = new TUpdatePeerWallpaper
                {
                    Peer = new TPeerUser { UserId = input.UserId },
                    Wallpaper = messageAction.Wallpaper,
                    WallpaperOverridden = true
                };

                await messageSender.PushMessageToPeerAsync(
                    new Peer(PeerType.User, otherUserId),
                    otherUserUpdate,
                    excludeUserId: input.UserId
                );
            }
        }

        return updates;
    }

    private async Task<IUpdates> RevertWallpaperAsync(IRequestInput input, long otherUserId)
    {
        // User reverts to previous wallpaper
        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Seq = 0
        };

        var updatePeerWallpaper = new TUpdatePeerWallpaper
        {
            Peer = new TPeerUser { UserId = otherUserId },
            Wallpaper = null, // Remove wallpaper
            WallpaperOverridden = false
        };

        updates.Updates.Add(updatePeerWallpaper);

        return updates;
    }

    private async Task<IWallPaper> GetWallpaperSchemaAsync(long wallpaperId, WallPaperSettings? settings)
    {
        var query = new GetWallpaperByIdQuery(wallpaperId);
        var wallpaper = await queryProcessor.ProcessAsync(query, CancellationToken.None);

        if (wallpaper == null)
        {
            return new TWallPaperNoFile { Id = wallpaperId };
        }

        return objectMapper.Map<IWallpaperReadModel, IWallPaper>(wallpaper);
    }

    private static long? GetWallpaperId(IInputWallPaper wallpaper)
    {
        return wallpaper switch
        {
            TInputWallPaper w => w.Id,
            TInputWallPaperNoFile w => w.Id,
            _ => null
        };
    }

    private static WallPaperSettings ToWallPaperSettings(IWallPaperSettings settings)
    {
        if (settings is TWallPaperSettings s)
        {
            return new WallPaperSettings(
                s.Blur,
                s.Motion,
                s.BackgroundColor,
                s.SecondBackgroundColor,
                s.ThirdBackgroundColor,
                s.FourthBackgroundColor,
                s.Intensity,
                s.Rotation,
                s.Emoticon
            );
        }

        return new WallPaperSettings(false, false, null, null, null, null, null, null, null);
    }
}
