// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Fetch popular <a href="https://corefork.telegram.org/api/bots/webapps#main-mini-apps">Main Mini Apps</a>, to be used in the <a href="https://corefork.telegram.org/api/search#apps-tab">apps tab of global search »</a>.
/// See <a href="https://corefork.telegram.org/method/bots.getPopularAppBots" />
///</summary>
internal sealed class GetPopularAppBotsHandler : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestGetPopularAppBots, MyTelegram.Schema.Bots.IPopularAppBots>,
    Bots.IGetPopularAppBotsHandler
{
    protected override Task<MyTelegram.Schema.Bots.IPopularAppBots> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestGetPopularAppBots obj)
    {
        // Return an empty list of popular app bots
        // In a real implementation, this would query a database or service for popular bots
        return Task.FromResult<MyTelegram.Schema.Bots.IPopularAppBots>(new MyTelegram.Schema.Bots.TPopularAppBots
        {
            Users = new TVector<MyTelegram.Schema.IUser>(),
            NextOffset = null // No pagination needed for empty list
        });
    }
}
