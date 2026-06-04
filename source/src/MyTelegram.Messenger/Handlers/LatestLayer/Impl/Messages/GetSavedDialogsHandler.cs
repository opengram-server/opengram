namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Returns the current saved dialog list, see <a href="https://corefork.telegram.org/api/saved-messages">here »</a> for more info.
/// See <a href="https://corefork.telegram.org/method/messages.getSavedDialogs" />
///</summary>
internal sealed class GetSavedDialogsHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetSavedDialogs, MyTelegram.Schema.Messages.ISavedDialogs>,
    Messages.IGetSavedDialogsHandler
{
    protected override Task<MyTelegram.Schema.Messages.ISavedDialogs> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetSavedDialogs obj)
    {
        return Task.FromResult<MyTelegram.Schema.Messages.ISavedDialogs>(new TSavedDialogs
        {
            Chats = [],
            Dialogs = [],
            Messages = [],
            Users = [],
        });
    }
}
