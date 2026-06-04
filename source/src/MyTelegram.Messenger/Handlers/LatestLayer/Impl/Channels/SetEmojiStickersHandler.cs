using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.Schema;
using MyTelegram.Messenger.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;

///<summary>
/// Set a custom emoji stickerset for supergroups. Only usable after reaching at least the boost level specified in the group_emoji_stickers_level_min config parameter.
/// See <a href="https://core.telegram.org/method/channels.setEmojiStickers" />
///</summary>
internal sealed class SetEmojiStickersHandler(
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IPeerHelper peerHelper,
    IAccessHashHelper accessHashHelper,
    ILogger<SetEmojiStickersHandler> logger) 
    : RpcResultObjectHandler<MyTelegram.Schema.Channels.RequestSetEmojiStickers, IBool>,
    Channels.ISetEmojiStickersHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Channels.RequestSetEmojiStickers obj)
    {
        logger.LogWarning(
            "*** SetEmojiStickers called by user {UserId} for channel {ChannelId} ***",
            input.UserId,
            obj.Channel);

        // Get channel and validate access
        var channel = peerHelper.GetChannel(obj.Channel);
        await accessHashHelper.CheckAccessHashAsync(input, obj.Channel);

        // Get sticker set ID from InputStickerSet
        long stickerSetId = 0;
        string? shortName = null;

        switch (obj.Stickerset)
        {
            case TInputStickerSetID inputId:
                stickerSetId = inputId.Id;
                break;
            case TInputStickerSetShortName inputShortName:
                shortName = inputShortName.ShortName;
                break;
            case TInputStickerSetEmpty:
                // Uninstall emoji pack - remove from channel
                logger.LogWarning("*** Uninstalling emoji pack for channel {ChannelId} ***", channel.PeerId);
                var uninstallCommand = new SetChannelEmojiStickersCommand(
                    ChannelId.Create(channel.PeerId),
                    input.ToRequestInfo(),
                    null // Remove sticker set
                );
                await commandBus.PublishAsync(uninstallCommand, CancellationToken.None);
                return new TBoolTrue();
            default:
                logger.LogWarning("Unsupported InputStickerSet type: {Type}", obj.Stickerset.GetType().Name);
                throw new RpcException(RpcErrors.RpcErrors400.StickersetInvalid);
        }

        // Check if this is a supergroup/channel (not basic group)
        if (channel.PeerType != MyTelegram.PeerType.Channel)
        {
            logger.LogWarning("Channel {ChannelId} is not a supergroup, type: {Type}", channel.PeerId, channel.PeerType);
            RpcErrors.RpcErrors400.ChatAdminRequired.ThrowRpcError();
        }

        // Query sticker set from DB
        var stickerSetReadModel = shortName != null
            ? await queryProcessor.ProcessAsync(new GetStickerSetByNameQuery(shortName), CancellationToken.None)
            : await queryProcessor.ProcessAsync(new GetStickerSetByIdQuery(stickerSetId), CancellationToken.None);

        if (stickerSetReadModel == null)
        {
            logger.LogWarning("Sticker set not found: id={Id}, shortName={ShortName}", stickerSetId, shortName);
            throw new RpcException(RpcErrors.RpcErrors406.StickersetInvalid);
        }

        if (!stickerSetReadModel.Emojis)
        {
            logger.LogWarning("Sticker set {Id} is not an emoji pack", stickerSetReadModel.StickerSetId);
            throw new RpcException(RpcErrors.RpcErrors400.StickersetInvalid);
        }

        // TODO: Check if channel has required boost level
        // This should check against group_emoji_stickers_level_min config
        // For now, we'll assume it's sufficient

        logger.LogWarning(
            "*** Installing emoji pack {StickerSetId} for channel {ChannelId} ***",
            stickerSetReadModel!.StickerSetId,
            channel.PeerId);

        // Create command to set emoji sticker set for channel
        var command = new SetChannelEmojiStickersCommand(
            ChannelId.Create(channel.PeerId),
            input.ToRequestInfo(),
            stickerSetReadModel.StickerSetId
        );

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogWarning(
            "*** Emoji pack {StickerSetId} installed for channel {ChannelId} successfully ***",
            stickerSetReadModel.StickerSetId,
            channel.PeerId);

        return new TBoolTrue();
    }
}
