namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Changes chat name and sends a service message on it.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHAT_ID_INVALID The provided chat id is invalid.
/// 400 CHAT_NOT_MODIFIED No changes were made to chat information because the new information you passed is identical to the current information.
/// 400 CHAT_TITLE_EMPTY No chat title provided.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.editChatTitle" />
///</summary>
internal sealed class EditChatTitleHandler(
    ICommandBus commandBus,
    IPeerHelper peerHelper,
    IRandomHelper randomHelper,
    IAccessHashHelper accessHashHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestEditChatTitle, MyTelegram.Schema.IUpdates>,
        Messages.IEditChatTitleHandler
{
    protected override async Task<IUpdates> HandleCoreAsync(IRequestInput input,
        RequestEditChatTitle obj)
    {
        if (string.IsNullOrEmpty(obj.Title))
        {
            throw new RpcException(RpcErrors.RpcErrors400.ChatTitleEmpty);
        }

        var peerType = peerHelper.GetPeerType(obj.ChatId);
        if (peerType == PeerType.Channel)
        {
            var command = new EditChannelTitleCommand(ChannelId.Create(obj.ChatId),
                input.ToRequestInfo(),
                obj.Title,
                new TMessageActionChatEditTitle { Title = obj.Title },
                randomHelper.NextInt64());
            await commandBus.PublishAsync(command, CancellationToken.None);
            return null!;
        }

        throw new RpcException(RpcErrors.RpcErrors400.ChatIdInvalid);
    }
}
