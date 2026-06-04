namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.getConnectedStarRefBots" />
///</summary>
internal sealed class GetConnectedStarRefBotsHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetConnectedStarRefBots, MyTelegram.Schema.Payments.IConnectedStarRefBots>,
    Payments.IGetConnectedStarRefBotsHandler
{
    protected override Task<MyTelegram.Schema.Payments.IConnectedStarRefBots> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetConnectedStarRefBots obj)
    {
        throw new NotImplementedException();
    }
}
