namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Notify the other user in a private chat that a screenshot of the chat was taken.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// 400 YOU_BLOCKED_USER You blocked this user.
/// See <a href="https://corefork.telegram.org/method/messages.sendScreenshotNotification" />
///</summary>
internal sealed class SendScreenshotNotificationHandler(
    IAccessHashHelper accessHashHelper,
    IPeerHelper peerHelper,
    IMessageAppService messageAppService,
    IRandomHelper randomHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSendScreenshotNotification, MyTelegram.Schema.IUpdates>,
        Messages.ISendScreenshotNotificationHandler
{
    protected override async Task<IUpdates> HandleCoreAsync(IRequestInput input,
        RequestSendScreenshotNotification obj)
    {
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);

        // Send a service message with messageActionScreenshotTaken
        var sendMessageInput = new SendMessageInput(
            input.ToRequestInfo(),
            input.UserId,
            peer,
            string.Empty,
            obj.RandomId,
            sendMessageType: SendMessageType.MessageService,
            messageType: MessageType.Unknown,
            messageAction: new TMessageActionScreenshotTaken(),
            inputReplyTo: obj.ReplyTo);

        await messageAppService.SendMessageAsync([sendMessageInput]);

        return null!;
    }
}
