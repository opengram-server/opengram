namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Gets the menu button action for a given user or for all users; only available for bots.
/// See <a href="https://corefork.telegram.org/method/bots.getBotMenuButton" />
///</summary>
internal sealed class GetBotMenuButtonHandler(
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestGetBotMenuButton, MyTelegram.Schema.IBotMenuButton>,
    Bots.IGetBotMenuButtonHandler
{
    protected override async Task<IBotMenuButton> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestGetBotMenuButton obj)
    {
        var botReadModel = await queryProcessor.ProcessAsync(
            new GetBotByUserIdQuery(input.UserId),
            CancellationToken.None);

        if (botReadModel == null)
        {
            return new TBotMenuButtonDefault();
        }

        // If the bot has a mini app URL configured, return a web app button
        if (!string.IsNullOrEmpty(botReadModel.MiniAppUrl))
        {
            return new TBotMenuButton
            {
                Text = "Menu",
                Url = botReadModel.MiniAppUrl
            };
        }

        // If the bot has commands, return commands button
        if (botReadModel.Commands.Count > 0)
        {
            return new TBotMenuButtonCommands();
        }

        return new TBotMenuButtonDefault();
    }
}
