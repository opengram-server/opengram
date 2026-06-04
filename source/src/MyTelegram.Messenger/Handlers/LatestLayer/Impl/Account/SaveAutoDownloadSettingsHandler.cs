namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Change media autodownload settings
/// See <a href="https://corefork.telegram.org/method/account.saveAutoDownloadSettings" />
///</summary>
internal sealed class SaveAutoDownloadSettingsHandler (ILogger<SaveAutoDownloadSettingsHandler> logger): RpcResultObjectHandler<MyTelegram.Schema.Account.RequestSaveAutoDownloadSettings, IBool>,
    Account.ISaveAutoDownloadSettingsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestSaveAutoDownloadSettings obj)
    {
        logger.LogInformation("SaveAutoDownloadSettingsHandler: {@Data}",obj);
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
