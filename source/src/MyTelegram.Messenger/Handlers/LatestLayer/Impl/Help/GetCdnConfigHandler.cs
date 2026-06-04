namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Help;

///<summary>
/// Get configuration for <a href="https://corefork.telegram.org/cdn">CDN</a> file downloads.
/// See <a href="https://corefork.telegram.org/method/help.getCdnConfig" />
///</summary>
internal sealed class GetCdnConfigHandler : RpcResultObjectHandler<MyTelegram.Schema.Help.RequestGetCdnConfig, MyTelegram.Schema.ICdnConfig>,
    Help.IGetCdnConfigHandler
{
    protected override Task<MyTelegram.Schema.ICdnConfig> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Help.RequestGetCdnConfig obj)
    {
        return Task.FromResult<MyTelegram.Schema.ICdnConfig>(new TCdnConfig
        {
            PublicKeys = []
        });
    }
}
