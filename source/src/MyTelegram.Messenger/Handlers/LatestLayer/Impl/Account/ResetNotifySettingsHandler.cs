namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Resets all notification settings from users and groups.
/// See <a href="https://corefork.telegram.org/method/account.resetNotifySettings" />
///</summary>
internal sealed class ResetNotifySettingsHandler : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestResetNotifySettings, IBool>,
    Account.IResetNotifySettingsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestResetNotifySettings obj)
    {
        // TODO: ResetNotifySettingsHandler
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
