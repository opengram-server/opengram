namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Set the default suggested admin rights for bots being added as admins to channels.
/// See <a href="https://corefork.telegram.org/method/bots.setBotBroadcastDefaultAdminRights" />
///</summary>
internal sealed class SetBotBroadcastDefaultAdminRightsHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetBotBroadcastDefaultAdminRights, IBool>,
    Bots.ISetBotBroadcastDefaultAdminRightsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetBotBroadcastDefaultAdminRights obj)
    {
        // Accept the default admin rights setting
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
