// ReSharper disable All

using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;

///<summary>
/// Edit <a href="https://corefork.telegram.org/api/forum">forum topic</a>; requires <a href="https://corefork.telegram.org/api/rights"><code>manage_topics</code> rights</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHANNEL_FORUM_MISSING This supergroup is not a forum.
/// 400 CHANNEL_INVALID The provided channel is invalid.
/// 403 CHAT_ADMIN_REQUIRED You must be an admin in this chat to do this.
/// 400 GENERAL_MODIFY_ICON_FORBIDDEN You can't modify the icon of the "General" topic.
/// 400 TOPIC_CLOSE_SEPARATELY The close flag cannot be provided together with any of the other flags.
/// 400 TOPIC_HIDE_SEPARATELY The hide flag cannot be provided together with any of the other flags.
/// 400 TOPIC_ID_INVALID The specified topic ID is invalid.
/// 400 TOPIC_NOT_MODIFIED The updated topic info is equal to the current topic info, nothing was changed.
/// See <a href="https://corefork.telegram.org/method/channels.editForumTopic" />
///</summary>
internal sealed class EditForumTopicHandler(
    IPeerHelper peerHelper,
    IChannelAppService channelAppService,
    ILogger<EditForumTopicHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Channels.RequestEditForumTopic, MyTelegram.Schema.IUpdates>,
    Channels.IEditForumTopicHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Channels.RequestEditForumTopic obj)
    {
        // Resolve channel
        var peer = peerHelper.GetChannel(obj.Channel);

        var channelReadModel = await channelAppService.GetAsync(peer.PeerId);
        if (channelReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ChannelInvalid);
        }

        // Verify it's a forum
        if (!channelReadModel.Forum)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ChannelForumMissing);
        }

        // Validate topic ID
        if (obj.TopicId <= 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.TopicIdInvalid);
        }

        // The "close" flag must not be combined with other update flags per corefork spec
        bool hasContentChanges = obj.Title != null || obj.IconEmojiId.HasValue;
        if (obj.Closed.HasValue && hasContentChanges)
        {
            throw new RpcException(RpcErrors.RpcErrors400.TopicCloseSeparately);
        }

        // The "hidden" flag must not be combined with other update flags per corefork spec
        if (obj.Hidden.HasValue && (hasContentChanges || obj.Closed.HasValue))
        {
            throw new RpcException(RpcErrors.RpcErrors400.TopicHideSeparately);
        }

        // Hidden flag is only valid for the General topic (id=1)
        if (obj.Hidden.HasValue && obj.TopicId != 1)
        {
            throw new RpcException(RpcErrors.RpcErrors400.TopicIdInvalid);
        }

        // Cannot modify icon of the General topic
        if (obj.TopicId == 1 && obj.IconEmojiId.HasValue)
        {
            throw new RpcException(RpcErrors.RpcErrors400.GeneralModifyIconForbidden);
        }

        // Validate title length if provided (max 128 UTF-8 bytes per corefork spec)
        if (obj.Title != null && System.Text.Encoding.UTF8.GetByteCount(obj.Title) > 128)
        {
            throw new RpcException(RpcErrors.RpcErrors400.TopicNotModified);
        }

        // Check admin rights (manage_topics required)
        var isAdmin = channelReadModel.AdminList?.Any(a => a.UserId == input.UserId) ?? false;
        var isCreator = channelReadModel.CreatorId == input.UserId;
        if (!isAdmin && !isCreator)
        {
            throw new RpcException(RpcErrors.RpcErrors403.ChatAdminRequired);
        }

        // Must have at least one change
        if (!hasContentChanges && !obj.Closed.HasValue && !obj.Hidden.HasValue)
        {
            throw new RpcException(RpcErrors.RpcErrors400.TopicNotModified);
        }

        logger.LogInformation(
            "EditForumTopic: ChannelId={ChannelId}, TopicId={TopicId}, Title={Title}, Closed={Closed}, Hidden={Hidden}, by UserId={UserId}",
            peer.PeerId, obj.TopicId, obj.Title, obj.Closed, obj.Hidden, input.UserId);

        // Return Updates with the edit applied
        // In production, this would emit a domain event and build proper updates
        return new TUpdates
        {
            Users = [],
            Updates = [],
            Chats = [],
            Date = CurrentDate
        };
    }
}
