// ReSharper disable All

using MyTelegram.Services.Services;
using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.Handlers.Messages;

///<summary>
/// Approve or reject a suggested post in a channel.
/// See <a href="https://corefork.telegram.org/method/messages.toggleSuggestedPostApproval" />
///</summary>
internal sealed class ToggleSuggestedPostApprovalHandler(
    IPeerHelper peerHelper,
    ISuggestedPostAppService suggestedPostAppService,
    IChannelAppService channelAppService,
    ILogger<ToggleSuggestedPostApprovalHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestToggleSuggestedPostApproval, MyTelegram.Schema.IUpdates>,
    Messages.IToggleSuggestedPostApprovalHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestToggleSuggestedPostApproval obj)
    {
        // Resolve peer (channel)
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        if (peer.PeerId == 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }

        // Validate message ID
        if (obj.MsgId <= 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.MsgIdInvalid);
        }

        // Verify channel exists and user has admin rights
        var channelReadModel = await channelAppService.GetAsync(peer.PeerId);
        if (channelReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ChannelInvalid);
        }

        var isAdmin = channelReadModel.AdminList?.Any(a => a.UserId == input.UserId) ?? false;
        var isCreator = channelReadModel.CreatorId == input.UserId;
        if (!isAdmin && !isCreator)
        {
            throw new RpcException(RpcErrors.RpcErrors403.ChatAdminRequired);
        }

        // Use MsgId as post identifier for the suggested post service
        var postId = $"{peer.PeerId}_{obj.MsgId}";

        if (obj.Reject)
        {
            // Reject the suggested post
            var rejectComment = obj.RejectComment ?? string.Empty;
            await suggestedPostAppService.RejectSuggestedPostAsync(postId, input.UserId, rejectComment);

            logger.LogInformation(
                "SuggestedPost rejected: PostId={PostId}, Channel={ChannelId}, By={UserId}, Reason={Reason}",
                postId, peer.PeerId, input.UserId, rejectComment);
        }
        else
        {
            // Approve the suggested post with optional schedule date
            await suggestedPostAppService.ApproveSuggestedPostAsync(
                postId,
                input.UserId,
                new MyTelegram.Domain.Shared.SuggestedPosts.SuggestedPostPrice());

            logger.LogInformation(
                "SuggestedPost approved: PostId={PostId}, Channel={ChannelId}, By={UserId}, ScheduleDate={ScheduleDate}",
                postId, peer.PeerId, input.UserId, obj.ScheduleDate);
        }

        return new TUpdates
        {
            Users = [],
            Updates = [],
            Chats = [],
            Date = CurrentDate
        };
    }
}
