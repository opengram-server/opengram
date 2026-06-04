namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get saved GIFs.
/// See <a href="https://corefork.telegram.org/method/messages.getSavedGifs" />
///</summary>
internal sealed class GetSavedGifsHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetSavedGifs, MyTelegram.Schema.Messages.ISavedGifs>,
    Messages.IGetSavedGifsHandler
{
    protected override Task<ISavedGifs> HandleCoreAsync(IRequestInput input,
        RequestGetSavedGifs obj)
    {
        return Task.FromResult<ISavedGifs>(new TSavedGifs { Gifs = [] });
    }
}
