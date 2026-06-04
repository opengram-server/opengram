// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.getSavedDialogsByID" />
///</summary>
internal sealed class GetSavedDialogsByIDHandler(ILogger<GetSavedDialogsByIDHandler> logger) : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetSavedDialogsByID, MyTelegram.Schema.Messages.ISavedDialogs>,
    Messages.IGetSavedDialogsByIDHandler
{
    protected override Task<MyTelegram.Schema.Messages.ISavedDialogs> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetSavedDialogsByID obj)
    {
        return Task.FromResult<MyTelegram.Schema.Messages.ISavedDialogs>(new TSavedDialogs
        {
            Chats = [],
            Dialogs = [],
            Messages = [],
            Users = []
        });
    }
}
