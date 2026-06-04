// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.Channels;

///<summary>
/// See <a href="https://corefork.telegram.org/method/channels.getMessageAuthor" />
///</summary>
internal sealed class GetMessageAuthorHandler : RpcResultObjectHandler<MyTelegram.Schema.Channels.RequestGetMessageAuthor, MyTelegram.Schema.IUser>,
    Channels.IGetMessageAuthorHandler
{
    protected override Task<MyTelegram.Schema.IUser> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Channels.RequestGetMessageAuthor obj)
    {
        throw new NotImplementedException();
    }
}
