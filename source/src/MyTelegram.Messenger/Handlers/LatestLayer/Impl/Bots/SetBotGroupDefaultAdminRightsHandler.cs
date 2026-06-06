namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Set the default suggested admin rights for bots being added as admins to groups.
/// See <a href="https://corefork.telegram.org/method/bots.setBotGroupDefaultAdminRights" />
///</summary>
internal sealed class SetBotGroupDefaultAdminRightsHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetBotGroupDefaultAdminRights, IBool>,
    Bots.ISetBotGroupDefaultAdminRightsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetBotGroupDefaultAdminRights obj)
    {
        // Accept the default admin rights setting
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
