// ReSharper disable All

using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Sends a custom request; for bots only
/// <para>Possible errors</para>
/// Code Type Description
/// 400 DATA_JSON_INVALID The provided JSON data is invalid.
/// 400 METHOD_INVALID The specified method is invalid.
/// 403 USER_BOT_INVALID User accounts must provide the <code>bot</code> method parameter when calling this method. If there is no such method parameter, this method can only be invoked by bot accounts.
/// 400 USER_BOT_REQUIRED This method can only be called by a bot.
/// See <a href="https://corefork.telegram.org/method/bots.sendCustomRequest" />
///</summary>
internal sealed class SendCustomRequestHandler(
    IPeerHelper peerHelper,
    ILogger<SendCustomRequestHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSendCustomRequest, MyTelegram.Schema.IDataJSON>,
    Bots.ISendCustomRequestHandler
{
    protected override Task<MyTelegram.Schema.IDataJSON> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSendCustomRequest obj)
    {
        // This method can only be called by bot accounts
        if (!peerHelper.IsBotUser(input.UserId))
        {
            throw new RpcException(RpcErrors.RpcErrors400.UserBotRequired);
        }

        // Validate custom method name
        if (string.IsNullOrWhiteSpace(obj.CustomMethod))
        {
            throw new RpcException(RpcErrors.RpcErrors400.MethodInvalid);
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
            "SendCustomRequest: Method={Method}, BotId={BotId}",
            obj.CustomMethod, input.UserId);

        // In production, this would forward the custom request to the bot's webhook URL.
        // The webhook processes the custom method and returns a response.
        // For now, return an empty JSON response as the webhook endpoint is not yet wired.
        return Task.FromResult<MyTelegram.Schema.IDataJSON>(new TDataJSON
        {
            Data = "{}"
        });
    }
}
