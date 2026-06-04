// ReSharper disable All

using MyTelegram.Schema.Users;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// See <a href="https://corefork.telegram.org/method/bots.getBotRecommendations" />
///</summary>
internal sealed class GetBotRecommendationsHandler : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestGetBotRecommendations, MyTelegram.Schema.Users.IUsers>,
    Bots.IGetBotRecommendationsHandler
{
    protected override Task<MyTelegram.Schema.Users.IUsers> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestGetBotRecommendations obj)
    {
        return Task.FromResult<MyTelegram.Schema.Users.IUsers>(new TUsers
        {
            Users = []
        });
    }
}
