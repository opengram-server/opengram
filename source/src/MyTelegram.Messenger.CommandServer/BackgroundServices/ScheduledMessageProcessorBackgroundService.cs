using EventFlow;
using Microsoft.Extensions.Hosting;
using MyTelegram.Queries;
using EventFlow.Queries;
using MyTelegram.EventFlow.Extensions;
using MyTelegram.Domain.Commands.Messaging;
using EventFlow.Commands;
using MyTelegram.Domain.Aggregates.Messaging;
using MyTelegram.Core;

namespace MyTelegram.Messenger.CommandServer.BackgroundServices;

/// <summary>
/// Background service that processes scheduled messages and sends them when their time comes
/// </summary>
public class ScheduledMessageProcessorBackgroundService(
    ILogger<ScheduledMessageProcessorBackgroundService> logger,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30); // Check every 30 seconds

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Scheduled message processor starting...");

        // Wait a bit before starting to allow other services to initialize
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        logger.LogInformation("Scheduled message processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessScheduledMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing scheduled messages");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        logger.LogInformation("Scheduled message processor stopped");
    }

    private async Task ProcessScheduledMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var queryProcessor = scope.ServiceProvider.GetRequiredService<IQueryProcessor>();
        var commandBus = scope.ServiceProvider.GetRequiredService<ICommandBus>();

        // Get current timestamp
        var currentTimestamp = DateTime.UtcNow.ToTimestamp();
        
        logger.LogWarning("*** Checking for scheduled messages at timestamp {Timestamp} ***", currentTimestamp);

        // Query all scheduled messages that should be sent now
        var scheduledMessages = await queryProcessor.ProcessAsync(
            new GetScheduleMessagesByDateQuery(currentTimestamp),
            cancellationToken);

        if (scheduledMessages == null || scheduledMessages.Count == 0)
        {
            logger.LogInformation("No scheduled messages found");
            return;
        }

        logger.LogWarning(
            "*** Found {Count} scheduled messages to send ***",
            scheduledMessages.Count);

        // Group messages by user and group (for grouped media)
        var groupedMessages = scheduledMessages
            .GroupBy(m => new { m.UserId, m.ToPeer, m.GroupId })
            .ToList();

        foreach (var group in groupedMessages)
        {
            try
            {
                var messages = group.ToList();
                var firstMessage = messages.First();

                logger.LogWarning(
                    "*** Sending {Count} scheduled messages for user {UserId} to peer {PeerType}:{PeerId} ***",
                    messages.Count,
                    firstMessage.UserId,
                    firstMessage.ToPeer.PeerType,
                    firstMessage.ToPeer.PeerId);

                // Send command to convert each scheduled message to regular message
                foreach (var message in messages)
                {
                    logger.LogWarning("*** Converting scheduled message {MessageId} for user {UserId} ***", 
                        message.MessageId, message.UserId);
                    
                    var command = new ConvertScheduledMessageToRegularCommand(
                        MessageId.Create(message.UserId, message.MessageId),
                        new RequestInfo(
                            ReqMsgId: 0,
                            UserId: message.UserId,
                            AccessHashKeyId: 0,
                            AuthKeyId: 0,
                            PermAuthKeyId: 0,
                            RequestId: Guid.NewGuid(),
                            Layer: 0,
                            Date: DateTime.UtcNow.ToTimestamp(),
                            DeviceType: DeviceType.Unknown,
                            AddRequestIdToCache: false,
                            IsSubRequest: true)
                    );

                    await commandBus.PublishAsync(command, cancellationToken);
                    logger.LogWarning("*** Command published for message {MessageId} ***", message.MessageId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error sending scheduled messages for user {UserId}",
                    group.Key.UserId);
            }
        }
    }
}
