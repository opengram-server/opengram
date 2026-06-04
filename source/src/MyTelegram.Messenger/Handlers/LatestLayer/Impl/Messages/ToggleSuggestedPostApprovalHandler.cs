// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.toggleSuggestedPostApproval" />
///</summary>
internal sealed class ToggleSuggestedPostApprovalHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestToggleSuggestedPostApproval, MyTelegram.Schema.IUpdates>,
    Messages.IToggleSuggestedPostApprovalHandler
{
    protected override Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestToggleSuggestedPostApproval obj)
    {
        throw new NotImplementedException();
    }
}
