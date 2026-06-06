namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Allow the specified bot to send us messages.
/// See <a href="https://corefork.telegram.org/method/bots.allowSendMessage" />
///</summary>
internal sealed class AllowSendMessageHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestAllowSendMessage, MyTelegram.Schema.IUpdates>,
    Bots.IAllowSendMessageHandler
{
    protected override Task<IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestAllowSendMessage obj)
    {
        // In a self-hosted server, bots can always send messages.
        // Return empty updates to acknowledge the permission grant.
        return Task.FromResult<IUpdates>(new TUpdates
        {
            Updates = [],
            Users = [],
            Chats = [],
            Date = CurrentDate
        });
    }
}
