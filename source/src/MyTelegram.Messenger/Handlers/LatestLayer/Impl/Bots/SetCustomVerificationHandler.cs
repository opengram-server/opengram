namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// See <a href="https://corefork.telegram.org/method/bots.setCustomVerification" />
///</summary>
internal sealed class SetCustomVerificationHandler : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetCustomVerification, IBool>,
    Bots.ISetCustomVerificationHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;

    public SetCustomVerificationHandler(ICommandBus commandBus, IQueryProcessor queryProcessor)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
    }

    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetCustomVerification obj)
    {
        // Get bot user id if specified, otherwise use current user
        long? botUserId = null;
        if (obj.Bot is TInputUser tInputUser)
        {
            var botUser = await _queryProcessor.ProcessAsync(
                new GetUserByIdQuery(tInputUser.UserId), 
                default);
            
            if (botUser == null || !botUser.Bot)
            {
                RpcErrors.RpcErrors400.BotInvalid.ThrowRpcError();
            }
            
            botUserId = tInputUser.UserId;
        }
        else if (obj.Bot != null)
        {
            RpcErrors.RpcErrors400.BotInvalid.ThrowRpcError();
        }

        // Check if peer is a user or channel
        if (obj.Peer is TInputPeerUser inputPeerUser)
        {
            if (obj.Enabled)
            {
                // Get bot verifier info
                var actualBotUserId = botUserId ?? input.UserId;
                var botVerifier = await _queryProcessor.ProcessAsync(
                    new GetBotVerifierByBotIdQuery(actualBotUserId), 
                    default);
                
                if (botVerifier == null)
                {
                    RpcErrors.RpcErrors400.BotInvalid.ThrowRpcError();
                }
                
                var description = botVerifier.CompanyName;
                if (!string.IsNullOrEmpty(obj.CustomDescription))
                {
                    if (!botVerifier.CanModifyCustomDescription)
                    {
                        RpcErrorsCustom.RpcErrors400.BotVerificationCannotModifyDescription.ThrowRpcError();
                    }
                    description = obj.CustomDescription;
                }
                
                var command = new SetCustomVerificationCommand(
                    UserId.Create(inputPeerUser.UserId),
                    input.ToRequestInfo(),
                    true,
                    botUserId,
                    inputPeerUser.UserId,
                    botVerifier.IconEmojiId,
                    description,
                    obj.CustomDescription);

                await _commandBus.PublishAsync(command, default);
            }
            else
            {
                var command = new SetCustomVerificationCommand(
                    UserId.Create(inputPeerUser.UserId),
                    input.ToRequestInfo(),
                    false,
                    botUserId,
                    inputPeerUser.UserId,
                    0,
                    "",
                    null);

                await _commandBus.PublishAsync(command, default);
            }
        }
        else if (obj.Peer is TInputPeerChannel inputPeerChannel)
        {
            if (obj.Enabled)
            {
                // Get bot verifier info
                var actualBotUserId = botUserId ?? input.UserId;
                var botVerifier = await _queryProcessor.ProcessAsync(
                    new GetBotVerifierByBotIdQuery(actualBotUserId), 
                    default);
                
                if (botVerifier == null)
                {
                    RpcErrors.RpcErrors400.BotInvalid.ThrowRpcError();
                }
                
                var description = botVerifier.CompanyName;
                if (!string.IsNullOrEmpty(obj.CustomDescription))
                {
                    if (!botVerifier.CanModifyCustomDescription)
                    {
                        RpcErrorsCustom.RpcErrors400.BotVerificationCannotModifyDescription.ThrowRpcError();
                    }
                    description = obj.CustomDescription;
                }
                
                var command = new SetChannelCustomVerificationCommand(
                    ChannelId.Create(inputPeerChannel.ChannelId),
                    input.ToRequestInfo(),
                    true,
                    botUserId,
                    inputPeerChannel.ChannelId,
                    botVerifier.IconEmojiId,
                    description,
                    obj.CustomDescription);

                await _commandBus.PublishAsync(command, default);
            }
            else
            {
                var command = new SetChannelCustomVerificationCommand(
                    ChannelId.Create(inputPeerChannel.ChannelId),
                    input.ToRequestInfo(),
                    false,
                    botUserId,
                    inputPeerChannel.ChannelId,
                    0,
                    "",
                    null);

                await _commandBus.PublishAsync(command, default);
            }
        }
        else
        {
            RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
        }

        return new TBoolTrue();
    }
}
