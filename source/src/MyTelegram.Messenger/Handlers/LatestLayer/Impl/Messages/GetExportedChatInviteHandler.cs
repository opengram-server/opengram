namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get info about a chat invite
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHANNEL_PRIVATE You haven't joined this channel/supergroup.
/// 400 CHAT_ADMIN_REQUIRED You must be an admin in this chat to do this.
/// 400 INVITE_HASH_EXPIRED The invite link has expired.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.getExportedChatInvite" />
///</summary>
internal sealed class GetExportedChatInviteHandler(
    IQueryProcessor queryProcessor,
    IAccessHashHelper accessHashHelper,
    IUserConverterService userConverterService,
    IChatInviteExportedConverterService chatInviteExportedConverterService)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetExportedChatInvite, MyTelegram.Schema.Messages.IExportedChatInvite>,
        Messages.IGetExportedChatInviteHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IExportedChatInvite> HandleCoreAsync(IRequestInput input,
        RequestGetExportedChatInvite obj)
    {
        if (obj.Peer is TInputPeerChannel inputPeerChannel)
        {
            await accessHashHelper.CheckAccessHashAsync(input, inputPeerChannel.ChannelId, inputPeerChannel.AccessHash, AccessHashType.Channel);
            var link = obj.Link.Substring(obj.Link.LastIndexOf("/") + 2);
            var chatInvite = await queryProcessor.ProcessAsync(new GetChatInviteQuery(inputPeerChannel.ChannelId, link));
            if (chatInvite == null)
            {
                throw new RpcException(new RpcError(400, "INVITE_HASH_EXPIRED"));
            }

            var users = await userConverterService.GetUserListAsync(input, new List<long> { chatInvite.AdminId }, false, false, input.Layer);
            var invite = chatInviteExportedConverterService.ToExportedChatInvite(chatInvite, input.Layer);

            return new MyTelegram.Schema.Messages.TExportedChatInvite
            {
                Invite = invite,
                Users = [.. users]
            };
        }

        throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
    }
}
