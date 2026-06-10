namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Start a conversation with a bot using a deep linking parameter.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 BOT_INVALID This is not a valid bot.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// 400 START_PARAM_EMPTY The start parameter is empty.
/// 400 START_PARAM_INVALID Start parameter invalid.
/// 400 START_PARAM_TOO_LONG Start parameter is too long.
/// See <a href="https://corefork.telegram.org/method/messages.startBot" />
///</summary>
internal sealed class StartBotHandler(
    IAccessHashHelper accessHashHelper,
    IPeerHelper peerHelper,
    IMessageAppService messageAppService,
    IRandomHelper randomHelper,
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestStartBot, MyTelegram.Schema.IUpdates>,
        Messages.IStartBotHandler
{
    protected override async Task<IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestStartBot obj)
    {
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);

        if (obj.StartParam != null && obj.StartParam.Length > 64)
        {
            throw new RpcException(new RpcError(400, "START_PARAM_TOO_LONG"));
        }

        // Validate the bot user
        var botPeer = peerHelper.GetPeer(obj.Bot, input.UserId);
        var botUser = await queryProcessor.ProcessAsync(new GetUserByIdQuery(botPeer.PeerId));
        if (botUser == null || !botUser.Bot)
        {
            throw new RpcException(new RpcError(400, "BOT_INVALID"));
        }

        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);

        // Build the /start message
        var message = string.IsNullOrEmpty(obj.StartParam)
            ? "/start"
            : $"/start {obj.StartParam}";

        var sendMessageInput = new SendMessageInput(
            input.ToRequestInfo(),
            input.UserId,
            peer,
            message,
            obj.RandomId);

        await messageAppService.SendMessageAsync([sendMessageInput]);

        return null!;
    }
}
