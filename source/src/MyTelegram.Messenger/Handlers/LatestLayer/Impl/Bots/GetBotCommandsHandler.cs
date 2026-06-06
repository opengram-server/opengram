namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Obtain a list of bot commands for the specified bot scope and language code.
/// See <a href="https://corefork.telegram.org/method/bots.getBotCommands" />
///</summary>
internal sealed class GetBotCommandsHandler(
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestGetBotCommands, TVector<MyTelegram.Schema.IBotCommand>>,
    Bots.IGetBotCommandsHandler
{
    protected override async Task<TVector<MyTelegram.Schema.IBotCommand>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestGetBotCommands obj)
    {
        var botReadModel = await queryProcessor.ProcessAsync(
            new GetBotByUserIdQuery(input.UserId),
            CancellationToken.None);

        if (botReadModel?.Commands == null || botReadModel.Commands.Count == 0)
        {
            return [];
        }

        var commands = botReadModel.Commands
            .Select(c => (MyTelegram.Schema.IBotCommand)new TBotCommand
            {
                Command = c.Command,
                Description = c.Description
            })
            .ToList();

        return new TVector<MyTelegram.Schema.IBotCommand>(commands);
    }
}
