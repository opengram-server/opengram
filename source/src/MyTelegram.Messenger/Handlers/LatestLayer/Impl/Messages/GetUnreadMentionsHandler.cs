namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get unread mentions.
/// See <a href="https://corefork.telegram.org/method/messages.getUnreadMentions" />
///</summary>
internal sealed class GetUnreadMentionsHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetUnreadMentions, MyTelegram.Schema.Messages.IMessages>,
    Messages.IGetUnreadMentionsHandler
{
    protected override Task<MyTelegram.Schema.Messages.IMessages> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetUnreadMentions obj)
    {
        // Return an empty message list - full mention tracking requires
        // cross-referencing with the mention index in the dialog.
        return Task.FromResult<MyTelegram.Schema.Messages.IMessages>(
            new MyTelegram.Schema.Messages.TMessages
            {
                Messages = [],
                Chats = [],
                Users = []
            });
    }
}
