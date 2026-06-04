using Microsoft.AspNetCore.Mvc;
using MyTelegram.BotApi.Services;
using MyTelegram.Domain.Shared.BotApi;

namespace MyTelegram.BotApi.Controllers;

/// <summary>
/// Внутренний API для приёма обновлений ботов от Query Server.
/// Наружу не публикуется и доступен только из внутренней сети.
/// </summary>
[ApiController]
[Route("internal/bot-updates")]
public class InternalBotUpdatesController(
    IUpdatesManager updatesManager,
    ILogger<InternalBotUpdatesController> logger) : ControllerBase
{
    [HttpPost("{botUserId}")]
    public async Task<IActionResult> ReceiveBotUpdate(
        long botUserId,
        [FromBody] BotUpdatePayload payload)
    {
        // Доступ по IP проверяет IpWhitelistMiddleware.
        // Эндпоинт должен быть доступен только из внутренней сети.

        try
        {
            logger.LogInformation("Received bot update for bot {BotId}: MessageId={MessageId}, SenderId={SenderId}",
                botUserId, payload.MessageId, payload.SenderUserId);

            // Формируем обновление в формате Bot API
            var update = new BotApiUpdate
            {
                // UpdateId генерируем через криптографический ГСЧ, а не Random.Shared
                UpdateId = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, int.MaxValue),
                Message = new BotApiMessage
                {
                    MessageId = payload.MessageId,
                    Date = payload.Date,
                    Chat = new BotApiChat
                    {
                        Id = payload.SenderUserId,
                        Type = "private"
                    },
                    From = new BotApiUser
                    {
                        Id = payload.SenderUserId,
                        IsBot = false,
                        FirstName = "User"
                    },
                    Text = payload.Text
                }
            };

            // Кладём обновление в очередь
            await updatesManager.AddUpdateAsync((int)botUserId, update);

            logger.LogInformation("Added update to queue for bot {BotId}", botUserId);

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error receiving bot update");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("callbacks/{botUserId}")]
    public async Task<IActionResult> ReceiveBotCallback(
        long botUserId,
        [FromBody] BotCallbackPayload payload)
    {
        try
        {
            logger.LogInformation("Received callback query for bot {BotId}: MessageId={MessageId}, FromUserId={FromUserId}, Data={Data}",
                botUserId, payload.MessageId, payload.FromUserId, payload.Data);

            // Формируем обновление callback_query в формате Bot API
            var update = new BotApiUpdate
            {
                // UpdateId генерируем через криптографический ГСЧ
                UpdateId = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, int.MaxValue),
                CallbackQuery = new BotApiCallbackQuery
                {
                    Id = Guid.NewGuid().ToString(),
                    From = new BotApiUser
                    {
                        Id = payload.FromUserId,
                        IsBot = false,
                        FirstName = "User"
                    },
                    Message = new BotApiMessage
                    {
                        MessageId = payload.MessageId,
                        Date = (int)payload.Timestamp,
                        Chat = new BotApiChat
                        {
                            Id = payload.FromUserId,
                            Type = "private"
                        }
                    },
                    ChatInstance = payload.FromUserId.ToString(),
                    Data = payload.Data
                }
            };

            // Кладём обновление в очередь
            await updatesManager.AddUpdateAsync((int)botUserId, update);

            logger.LogInformation("Added callback_query update to queue for bot {BotId}", botUserId);

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error receiving callback query");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public record BotUpdatePayload(
    int MessageId,
    long SenderUserId,
    string Text,
    int Date
);

public record BotCallbackPayload(
    long FromUserId,
    int MessageId,
    string? Data,
    long Timestamp
);
