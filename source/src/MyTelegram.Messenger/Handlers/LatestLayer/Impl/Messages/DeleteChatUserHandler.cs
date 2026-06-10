namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Deletes a user from a chat and sends a service message on it.
/// See <a href="https://corefork.telegram.org/method/messages.deleteChatUser" />
///</summary>
internal sealed class DeleteChatUserHandler(
    ICommandBus commandBus,
    IPeerHelper peerHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestDeleteChatUser, MyTelegram.Schema.IUpdates>,
        Messages.IDeleteChatUserHandler
{
    protected override async Task<IUpdates> HandleCoreAsync(IRequestInput input,
        RequestDeleteChatUser obj)
    {
        var peerType = peerHelper.GetPeerType(obj.ChatId);
        if (peerType == PeerType.Channel)
        {
            var peer = peerHelper.GetPeer(obj.UserId, input.UserId);
            var command = new LeaveChannelCommand(
                ChannelMemberId.Create(obj.ChatId, peer.PeerId),
                input.ToRequestInfo(),
                obj.ChatId,
                peer.PeerId);
            await commandBus.PublishAsync(command);
            return null!;
        }

        throw new RpcException(RpcErrors.RpcErrors400.ChatIdInvalid);
    }
}
