namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;

///<summary>
/// Get the author of a channel message.
/// See <a href="https://corefork.telegram.org/method/channels.getMessageAuthor" />
///</summary>
internal sealed class GetMessageAuthorHandler(
    IQueryProcessor queryProcessor,
    IAccessHashHelper accessHashHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Channels.RequestGetMessageAuthor, MyTelegram.Schema.IUser>,
        MyTelegram.Messenger.Handlers.Channels.IGetMessageAuthorHandler
{
    protected override async Task<IUser> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Channels.RequestGetMessageAuthor obj)
    {
        // This method requires channel admin rights and returns the actual author
        // of an anonymous admin message. Not commonly used - return invalid for now.
        throw new RpcException(RpcErrors.RpcErrors400.ChannelInvalid);
    }
}
