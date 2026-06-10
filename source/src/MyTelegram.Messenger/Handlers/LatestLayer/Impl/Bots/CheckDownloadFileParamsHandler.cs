namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Check if a bot can download a file.
/// See <a href="https://corefork.telegram.org/method/bots.checkDownloadFileParams" />
///</summary>
internal sealed class CheckDownloadFileParamsHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestCheckDownloadFileParams, IBool>,
        Bots.ICheckDownloadFileParamsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestCheckDownloadFileParams obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
