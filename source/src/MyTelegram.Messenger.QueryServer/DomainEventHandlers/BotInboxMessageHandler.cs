using MyTelegram.Domain.Events.Messaging;
using MyTelegram.Domain.Aggregates.Messaging;
using EventFlow.Aggregates;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Services.Services;
using MyTelegram.Schema.Messages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace MyTelegram.Messenger.QueryServer.DomainEventHandlers;

/// <summary>
/// Обрабатывает входящие сообщения для ботов и уведомляет Bot API по HTTP
/// </summary>
public class BotInboxMessageHandler(
    IQueryProcessor queryProcessor,
    IHttpClientFactory httpClientFactory,
    ILogger<BotInboxMessageHandler> logger) 
    : ISubscribeSynchronousTo<MessageAggregate, MessageId, InboxMessageCreatedEvent>
{
    public async Task HandleAsync(IDomainEvent<MessageAggregate, MessageId, InboxMessageCreatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            var messageItem = domainEvent.AggregateEvent.InboxMessageItem;

            // Проверяем, является ли получатель ботом
            var userReadModel = await queryProcessor.ProcessAsync(
                new GetUserByIdQuery(messageItem.OwnerPeer.PeerId),
                CancellationToken.None);

            if (userReadModel == null || !userReadModel.Bot)
            {
                return; // Не бот, пропускаем
            }

            // Пропускаем сообщения ОТ самого бота, чтобы не уйти в бесконечный цикл
            if (messageItem.SenderPeer.PeerId == messageItem.OwnerPeer.PeerId)
            {
                logger.LogDebug("Skipping message from bot {BotId} to itself", messageItem.OwnerPeer.PeerId);
                return;
            }

            logger.LogInformation("Inbox message created for bot {BotId}, MessageId={MessageId}, from User {SenderId}",
                messageItem.OwnerPeer.PeerId, messageItem.MessageId, messageItem.SenderPeer.PeerId);

            // Уведомляем Bot API по HTTP
            await NotifyBotApiAsync(messageItem);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling inbox message for bot");
        }
    }

    private async Task NotifyBotApiAsync(MessageItem messageItem)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient();

            // Внутренний endpoint Bot API (наружу не выставляется)
            var botApiUrl = $"http://bot-api-server:8081/internal/bot-updates/{messageItem.OwnerPeer.PeerId}";

            var payload = new
            {
                message_id = messageItem.MessageId,
                sender_user_id = messageItem.SenderPeer.PeerId,
                text = messageItem.Message,
                date = messageItem.Date
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(botApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Notified Bot API about message {MessageId} for bot {BotId}",
                    messageItem.MessageId, messageItem.OwnerPeer.PeerId);
            }
            else
            {
                logger.LogWarning("Failed to notify Bot API: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error notifying Bot API for bot {BotId}", messageItem.OwnerPeer.PeerId);
        }
    }
}
