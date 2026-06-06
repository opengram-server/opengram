namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Allow the specified bot to send us messages.
/// See <a href="https://corefork.telegram.org/method/bots.allowSendMessage" />
///</summary>
internal sealed class AllowSendMessageHandler(
    IPtsHelper ptsHelper,
    IUserAppService userAppService,
    ILogger<AllowSendMessageHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestAllowSendMessage, MyTelegram.Schema.IUpdates>,
    Bots.IAllowSendMessageHandler
{
    protected override async Task<IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestAllowSendMessage obj)
    {
        long botUserId = 0;
        if (obj.Bot is TInputUser inputUser)
        {
            botUserId = inputUser.UserId;
        }

        if (botUserId == 0)
        {
            throw new RpcException(new RpcError(400, "BOT_INVALID"));
        }

        // Verify the target is actually a bot
        var botReadModel = await userAppService.GetAsync(botUserId);
        if (botReadModel == null || !botReadModel.Bot)
        {
            throw new RpcException(new RpcError(400, "BOT_INVALID"));
        }

        logger.LogDebug("AllowSendMessage: User {UserId} allowed bot {BotUserId} to send messages",
            input.UserId, botUserId);

        // Produce an update indicating the bot can now send messages
        var pts = ptsHelper.GetCachedPts(input.UserId);

        return new TUpdates
        {
            Updates = new TVector<IUpdate>(new List<IUpdate>
            {
                new TUpdatePeerSettings
                {
                    Peer = new TPeerUser { UserId = botUserId },
                    Settings = new MyTelegram.Schema.TPeerSettings
                    {
                        ReportSpam = false,
                        AddContact = false,
                        BlockContact = false,
                        ShareContact = false,
                        NeedContactsException = false,
                        Autoarchived = false,
                    }
                }
            }),
            Users = new TVector<IUser>(),
            Chats = [],
            Date = CurrentDate
        };
    }
}
