namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Get a list of bots owned by the current user.
/// See <a href="https://corefork.telegram.org/method/bots.getAdminedBots" />
///</summary>
internal sealed class GetAdminedBotsHandler(
    IQueryProcessor queryProcessor,
    IUserAppService userAppService,
    ILogger<GetAdminedBotsHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestGetAdminedBots, TVector<MyTelegram.Schema.IUser>>,
    Bots.IGetAdminedBotsHandler
{
    protected override async Task<TVector<IUser>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestGetAdminedBots obj)
    {
        // Query all bots owned by the current user
        var ownedBots = await queryProcessor.ProcessAsync(
            new GetBotsByOwnerUserIdQuery(input.UserId));

        if (ownedBots.Count == 0)
        {
            return [];
        }

        // Load full user read models for each bot
        var botUserIds = ownedBots.Select(b => b.UserId).ToList();
        var userReadModels = await userAppService.GetListAsync(botUserIds);

        // Convert to TUser objects
        var users = new List<IUser>();
        foreach (var userReadModel in userReadModels)
        {
            users.Add(new TUser
            {
                Id = userReadModel.UserId,
                Bot = true,
                FirstName = userReadModel.FirstName,
                LastName = userReadModel.LastName,
                Username = userReadModel.UserName,
                AccessHash = userReadModel.AccessHash,
                Phone = userReadModel.PhoneNumber,
                Status = new TUserStatusRecently(),
                BotInfoVersion = 1
            });
        }

        logger.LogDebug("GetAdminedBots: Found {Count} bots for UserId={UserId}", users.Count, input.UserId);

        return new TVector<IUser>(users);
    }
}
