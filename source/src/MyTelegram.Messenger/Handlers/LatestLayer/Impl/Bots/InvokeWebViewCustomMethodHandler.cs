// ReSharper disable All

using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Send a custom request from a <a href="https://corefork.telegram.org/api/bots/webapps">mini bot app</a>, triggered by a <a href="https://corefork.telegram.org/api/web-events#web-app-invoke-custom-method">web_app_invoke_custom_method event »</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 BOT_INVALID This is not a valid bot.
/// 400 DATA_JSON_INVALID The provided JSON data is invalid.
/// See <a href="https://corefork.telegram.org/method/bots.invokeWebViewCustomMethod" />
///</summary>
internal sealed class InvokeWebViewCustomMethodHandler(
    IPeerHelper peerHelper,
    IUserAppService userAppService,
    ILogger<InvokeWebViewCustomMethodHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestInvokeWebViewCustomMethod, MyTelegram.Schema.IDataJSON>,
    Bots.IInvokeWebViewCustomMethodHandler
{
    protected override async Task<MyTelegram.Schema.IDataJSON> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestInvokeWebViewCustomMethod obj)
    {
        // Resolve bot user
        var botPeer = peerHelper.GetPeer(obj.Bot, input.UserId);
        if (botPeer.PeerId == 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.BotInvalid);
        }

        // Verify the target is actually a bot
        if (!peerHelper.IsBotUser(botPeer.PeerId))
        {
            throw new RpcException(RpcErrors.RpcErrors400.BotInvalid);
        }

        // Verify bot exists
        var botUser = await userAppService.GetAsync(botPeer.PeerId);
        if (botUser == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.BotInvalid);
        }

        // Validate custom method name
        if (string.IsNullOrWhiteSpace(obj.CustomMethod))
        {
            throw new RpcException(RpcErrors.RpcErrors400.DataJsonInvalid);
        }

        // Validate JSON params
        if (obj.Params == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.DataJsonInvalid);
        }

        if (obj.Params is MyTelegram.Schema.TDataJSON dataJson)
        {
            if (string.IsNullOrWhiteSpace(dataJson.Data))
            {
                throw new RpcException(RpcErrors.RpcErrors400.DataJsonInvalid);
            }

            // Validate JSON syntax
            try
            {
                System.Text.Json.JsonDocument.Parse(dataJson.Data);
            }
            catch (System.Text.Json.JsonException)
            {
                throw new RpcException(RpcErrors.RpcErrors400.DataJsonInvalid);
            }
        }

        logger.LogInformation(
            "InvokeWebViewCustomMethod: Bot={BotId}, Method={Method}, UserId={UserId}",
            botPeer.PeerId, obj.CustomMethod, input.UserId);

        // In production, this forwards the custom method call to the bot's backend.
        // The bot processes it and the response is sent back via custom_method_invoked event.
        // Return an empty JSON result as the bot backend integration is not yet wired.
        return new TDataJSON
        {
            Data = "{}"
        };
    }
}
