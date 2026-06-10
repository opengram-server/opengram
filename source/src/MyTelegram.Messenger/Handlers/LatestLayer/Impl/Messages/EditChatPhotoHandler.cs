namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Changes chat photo and sends a service message on it.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHAT_ID_INVALID The provided chat id is invalid.
/// 400 CHAT_NOT_MODIFIED No changes were made to chat information because the new information you passed is identical to the current information.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// 400 PHOTO_CROP_SIZE_SMALL Photo is too small.
/// 400 PHOTO_EXT_INVALID The extension of the photo is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.editChatPhoto" />
///</summary>
internal sealed class EditChatPhotoHandler(
    ICommandBus commandBus,
    IPeerHelper peerHelper,
    IRandomHelper randomHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestEditChatPhoto, MyTelegram.Schema.IUpdates>,
        Messages.IEditChatPhotoHandler
{
    protected override async Task<IUpdates> HandleCoreAsync(IRequestInput input,
        RequestEditChatPhoto obj)
    {
        var peerType = peerHelper.GetPeerType(obj.ChatId);
        if (peerType == PeerType.Channel)
        {
            long? photoId = null;
            if (obj.Photo is TInputChatPhotoEmpty)
            {
                photoId = null;
            }

            var command = new EditChannelPhotoCommand(ChannelId.Create(obj.ChatId),
                input.ToRequestInfo(),
                photoId,
                new TMessageActionChatEditPhoto { Photo = new TPhotoEmpty { Id = 0 } },
                randomHelper.NextInt64());
            await commandBus.PublishAsync(command, CancellationToken.None);
            return null!;
        }

        throw new RpcException(RpcErrors.RpcErrors400.ChatIdInvalid);
    }
}
