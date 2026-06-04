namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Smsjobs;

///<summary>
/// Enable SMS jobs (official clients only).
/// See <a href="https://corefork.telegram.org/method/smsjobs.join" />
///</summary>
internal sealed class JoinHandler : RpcResultObjectHandler<MyTelegram.Schema.Smsjobs.RequestJoin, IBool>,
    Smsjobs.IJoinHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Smsjobs.RequestJoin obj)
    {
        throw new NotImplementedException();
    }
}
