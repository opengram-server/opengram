namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Get localized name, about text and description of a bot (or of the current account, if called by a bot).
/// See <a href="https://corefork.telegram.org/method/bots.getBotInfo" />
///</summary>
internal sealed class GetBotInfoHandler(
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestGetBotInfo, MyTelegram.Schema.Bots.IBotInfo>,
    Bots.IGetBotInfoHandler
{
    protected override async Task<MyTelegram.Schema.Bots.IBotInfo> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestGetBotInfo obj)
    {
        // Determine which bot to query — either the bot specified or the calling bot itself
        long botUserId = input.UserId;
        if (obj.Bot is TInputUser inputUser)
        {
            botUserId = inputUser.UserId;
        }

        var botReadModel = await queryProcessor.ProcessAsync(
            new GetBotByUserIdQuery(botUserId),
            CancellationToken.None);

        if (botReadModel == null)
        {
            RpcErrors.RpcErrors400.UserBotInvalid.ThrowRpcError();
        }

        return new MyTelegram.Schema.Bots.TBotInfo
        {
            Name = botReadModel!.BotName,
            About = botReadModel.About ?? string.Empty,
            Description = botReadModel.Description ?? string.Empty
        };
    }
}
