namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get count of online users in a chat.
/// See <a href="https://corefork.telegram.org/method/messages.getOnlines" />
///</summary>
internal sealed class GetOnlinesHandler(
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetOnlines, MyTelegram.Schema.IChatOnlines>,
    Messages.IGetOnlinesHandler
{
    protected override async Task<IChatOnlines> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetOnlines obj)
    {
        // Resolve peer to get member count
        long peerId = 0;
        if (obj.Peer is TInputPeerChat inputPeerChat)
        {
            peerId = inputPeerChat.ChatId;
        }
        else if (obj.Peer is TInputPeerChannel inputPeerChannel)
        {
            peerId = inputPeerChannel.ChannelId;
        }

        int onlineCount = 1; // At least the requesting user

        if (peerId > 0)
        {
            try
            {
                var members = await queryProcessor.ProcessAsync(
                    new GetChatMemberListQuery(peerId),
                    CancellationToken.None);

                if (members != null && members.Count > 0)
                {
                    // Approximate: count each member as online
                    // (a full implementation would check each user's online status)
                    onlineCount = members.Count;
                }
            }
            catch
            {
                // Fallback
            }
        }

        return new TChatOnlines
        {
            Onlines = onlineCount
        };
    }
}
