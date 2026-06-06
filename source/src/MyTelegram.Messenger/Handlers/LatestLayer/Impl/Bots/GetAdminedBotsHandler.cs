namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Get a list of bots owned by the current user.
/// See <a href="https://corefork.telegram.org/method/bots.getAdminedBots" />
///</summary>
internal sealed class GetAdminedBotsHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestGetAdminedBots, TVector<MyTelegram.Schema.IUser>>,
    Bots.IGetAdminedBotsHandler
{
    protected override Task<TVector<IUser>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestGetAdminedBots obj)
    {
        // Bot ownership tracking requires a dedicated query across the bot read model.
        // For now, return an empty list. When GetBotsByOwnerUserIdQuery is implemented,
        // this can be expanded.
        return Task.FromResult<TVector<IUser>>([]);
    }
}
