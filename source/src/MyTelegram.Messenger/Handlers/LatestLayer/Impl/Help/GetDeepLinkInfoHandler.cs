namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Help;

///<summary>
/// Get info about an unsupported deep link, see here for more info.
/// See <a href="https://corefork.telegram.org/method/help.getDeepLinkInfo" />
///</summary>
internal sealed class GetDeepLinkInfoHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Help.RequestGetDeepLinkInfo, MyTelegram.Schema.Help.IDeepLinkInfo>,
    Help.IGetDeepLinkInfoHandler
{
    protected override Task<MyTelegram.Schema.Help.IDeepLinkInfo> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Help.RequestGetDeepLinkInfo obj)
    {
        // For a self-hosted server, deep link info is not applicable.
        // Return empty to indicate no special deep link info is available.
        return Task.FromResult<MyTelegram.Schema.Help.IDeepLinkInfo>(
            new MyTelegram.Schema.Help.TDeepLinkInfoEmpty());
    }
}
