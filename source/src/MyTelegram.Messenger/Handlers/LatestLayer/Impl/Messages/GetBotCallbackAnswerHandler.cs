using MyTelegram.Schema.Messages;
using MyTelegram.Services.Services;
using MyTelegram.ReadModel.Interfaces;
using System.Text.Json;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Нажимает inline-кнопку обратного вызова и получает ответ от бота
/// <para>Possible errors</para>
/// Code Type Description
/// 400 BOT_RESPONSE_TIMEOUT A timeout occurred while fetching data from the bot.
/// 400 CHANNEL_INVALID The provided channel is invalid.
/// 400 CHANNEL_PRIVATE You haven't joined this channel/supergroup.
/// 400 DATA_INVALID Encrypted data invalid.
/// 400 MESSAGE_ID_INVALID The provided message id is invalid.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// -503 Timeout Timeout while fetching data.
/// See <a href="https://corefork.telegram.org/method/messages.getBotCallbackAnswer" />
///</summary>
internal sealed class GetBotCallbackAnswerHandler(
    IQueryProcessor queryProcessor,
    IHttpClientFactory httpClientFactory,
    ILogger<GetBotCallbackAnswerHandler> logger) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetBotCallbackAnswer, IBotCallbackAnswer>,
    Messages.IGetBotCallbackAnswerHandler
{
    protected override async Task<IBotCallbackAnswer> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetBotCallbackAnswer obj)
    {
        try
        {
            // Получаем идентификатор бота из peer
            var botUserId = obj.Peer switch
            {
                TInputPeerUser user => user.UserId,
                _ => 0L
            };

            if (botUserId == 0)
            {
                logger.LogWarning("Invalid peer type for callback query");
                return CreateEmptyAnswer();
            }

            // Проверяем, что адресат — бот
            var userReadModel = await queryProcessor.ProcessAsync(
                new GetUserByIdQuery(botUserId),
                CancellationToken.None);

            if (userReadModel == null || !userReadModel.Bot)
            {
                logger.LogWarning("User {UserId} is not a bot", botUserId);
                return CreateEmptyAnswer();
            }

            logger.LogInformation("Callback query from user {UserId} to bot {BotId}, MsgId={MsgId}, Data={Data}",
                input.UserId, botUserId, obj.MsgId, obj.Data.HasValue ? BitConverter.ToString(obj.Data.Value.ToArray()) : "null");

            // Отправляем callback_query в Bot API по HTTP без ожидания, чтобы ответить клиенту сразу
            _ = Task.Run(async () =>
            {
                try
                {
                    await NotifyBotApiAsync(botUserId, input.UserId, obj.MsgId, obj.Data.HasValue ? obj.Data.Value.ToArray() : null);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error notifying Bot API about callback query");
                }
            });

            // Сразу возвращаем пустой ответ (бот ответит позже через answerCallbackQuery)
            return CreateEmptyAnswer();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling callback query");
            return CreateEmptyAnswer();
        }
    }

    private async Task NotifyBotApiAsync(long botUserId, long fromUserId, int messageId, byte[]? data)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient();
            var botApiUrl = $"http://bot-api-server:8081/internal/bot-updates/callbacks/{botUserId}";

            var payload = new
            {
                from_user_id = fromUserId,
                message_id = messageId,
                data = data != null ? System.Text.Encoding.UTF8.GetString(data) : null,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(botApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Notified Bot API about callback query for bot {BotId}", botUserId);
            }
            else
            {
                logger.LogWarning("Failed to notify Bot API: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error notifying Bot API about callback query");
        }
    }

    private static TBotCallbackAnswer CreateEmptyAnswer()
    {
        return new TBotCallbackAnswer
        {
            Alert = false,
            HasUrl = false,
            NativeUi = false,
            Message = "",
            CacheTime = 0
        };
    }
}
