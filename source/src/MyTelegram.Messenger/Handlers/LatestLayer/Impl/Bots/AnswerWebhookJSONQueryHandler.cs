// ReSharper disable All

using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Answers a custom query; for bots only
/// <para>Possible errors</para>
/// Code Type Description
/// 400 DATA_JSON_INVALID The provided JSON data is invalid.
/// 400 QUERY_ID_INVALID The query ID is invalid.
/// 403 USER_BOT_INVALID User accounts must provide the <code>bot</code> method parameter when calling this method. If there is no such method parameter, this method can only be invoked by bot accounts.
/// 400 USER_BOT_REQUIRED This method can only be called by a bot.
/// See <a href="https://corefork.telegram.org/method/bots.answerWebhookJSONQuery" />
///</summary>
internal sealed class AnswerWebhookJSONQueryHandler(
    IPeerHelper peerHelper,
    ILogger<AnswerWebhookJSONQueryHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestAnswerWebhookJSONQuery, IBool>,
    Bots.IAnswerWebhookJSONQueryHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestAnswerWebhookJSONQuery obj)
    {
        // This method can only be called by bot accounts
        if (!peerHelper.IsBotUser(input.UserId))
        {
            throw new RpcException(RpcErrors.RpcErrors403.UserBotInvalid);
        }

        // Validate query ID
        if (obj.QueryId == 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.QueryIdInvalid);
        }

        // Validate response data
        if (obj.Data == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.DataJsonInvalid);
        }

        if (obj.Data is MyTelegram.Schema.TDataJSON dataJson)
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
            "AnswerWebhookJSONQuery: QueryId={QueryId}, BotId={BotId}",
            obj.QueryId, input.UserId);

        // In production, this stores the answer and resolves the pending webhook query.
        // The bot framework would match QueryId to a pending request and deliver the response.
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
