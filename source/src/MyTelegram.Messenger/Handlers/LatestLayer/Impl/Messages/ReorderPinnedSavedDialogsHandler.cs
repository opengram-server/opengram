namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Reorder pinned <a href="https://corefork.telegram.org/api/saved-messages">saved message dialogs »</a>.
/// See <a href="https://corefork.telegram.org/method/messages.reorderPinnedSavedDialogs" />
///</summary>
internal sealed class ReorderPinnedSavedDialogsHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestReorderPinnedSavedDialogs, IBool>,
    Messages.IReorderPinnedSavedDialogsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestReorderPinnedSavedDialogs obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
