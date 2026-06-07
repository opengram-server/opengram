namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Help;

///<summary>
/// Get passport configuration.
/// See <a href="https://corefork.telegram.org/method/help.getPassportConfig" />
///</summary>
internal sealed class GetPassportConfigHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Help.RequestGetPassportConfig, MyTelegram.Schema.Help.IPassportConfig>,
    Help.IGetPassportConfigHandler
{
    protected override Task<MyTelegram.Schema.Help.IPassportConfig> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Help.RequestGetPassportConfig obj)
    {
        // Passport is not supported on self-hosted servers.
        // Return the not-modified variant to indicate nothing has changed.
        return Task.FromResult<MyTelegram.Schema.Help.IPassportConfig>(
            new MyTelegram.Schema.Help.TPassportConfigNotModified());
    }
}
