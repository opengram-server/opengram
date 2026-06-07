namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get unread mentions.
/// See <a href="https://corefork.telegram.org/method/messages.getUnreadMentions" />
///</summary>
internal sealed class GetUnreadMentionsHandler(
    IPeerHelper peerHelper,
    IAccessHashHelper accessHashHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetUnreadMentions, MyTelegram.Schema.Messages.IMessages>,
    Messages.IGetUnreadMentionsHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IMessages> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetUnreadMentions obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);

        // Mention tracking requires a dedicated MentionReadModel.
        // Return an empty message list — clients will display no unread mentions.
        return new MyTelegram.Schema.Messages.TMessages
        {
            Messages = [],
            Chats = [],
            Users = []
        };
    }
}
