using MyTelegram.Core;
using Microsoft.Extensions.Logging;

namespace MyTelegram.Messenger.QueryServer.Handlers;

/// <summary>
/// Handler for GetCustomEmojiDocuments when routed as Download request
/// This fixes the issue where file-server returns empty documents
/// </summary>
public class GetCustomEmojiDocumentsDownloadHandler(
    IMessageQueueProcessor<MessengerQueryDataReceivedEvent> messengerQueryProcessor,
    IMessageQueueProcessor<DownloadDataReceivedEvent> downloadProcessor,
    ILogger<GetCustomEmojiDocumentsDownloadHandler> logger)
    : IEventHandler<DownloadDataReceivedEvent>, ITransientDependency
{
    public Task HandleEventAsync(DownloadDataReceivedEvent eventData)
    {
        // Check if this is GetCustomEmojiDocuments request (0xd9ab0f54)
        if (eventData.ObjectId != 0xd9ab0f54)
        {
            // Not GetCustomEmojiDocuments - forward to file-server for normal processing
            downloadProcessor.Enqueue(eventData, eventData.PermAuthKeyId);
            return Task.CompletedTask;
        }

        // This IS GetCustomEmojiDocuments - handle it ourselves, DON'T forward to file-server
        logger.LogWarning("*** Intercepted GetCustomEmojiDocuments in DownloadHandler! UserId={UserId} ***", eventData.UserId);

        // Convert to MessengerQueryDataReceivedEvent and process
        var queryEvent = new MessengerQueryDataReceivedEvent(
            eventData.ConnectionId,
            eventData.RequestId,
            eventData.ObjectId,
            eventData.UserId,
            eventData.ReqMsgId,
            eventData.SeqNumber,
            eventData.AuthKeyId,
            eventData.PermAuthKeyId,
            eventData.Data,
            eventData.Layer,
            eventData.Date,
            eventData.DeviceType,
            eventData.ClientIp,
            eventData.SessionId,
            0  // AccessHashKeyId - not used in v0.32.206.802
        );

        // Enqueue as query event to be processed by MessengerEventHandler
        messengerQueryProcessor.Enqueue(queryEvent, eventData.PermAuthKeyId);
        
        return Task.CompletedTask;
    }
}
