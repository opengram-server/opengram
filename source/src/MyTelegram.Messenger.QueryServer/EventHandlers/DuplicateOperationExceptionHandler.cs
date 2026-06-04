using MyTelegram.Schema.Extensions;

namespace MyTelegram.Messenger.QueryServer.EventHandlers;

public class DuplicateOperationExceptionHandler(
    IQueryProcessor queryProcessor,
    IObjectMessageSender messageSender,
    ILogger<DuplicateOperationExceptionHandler> logger)
    : IEventHandler<DuplicateCommandEvent>, ITransientDependency
{
    public async Task HandleEventAsync(DuplicateCommandEvent eventData)
    {
        logger.LogWarning("Duplicate command, userId: {UserId} reqMsgId: {ReqMsgId}", eventData.UserId, eventData.ReqMsgId);
        
        // Try to find cached RPC result
        var rpcResultReadModel = await queryProcessor.ProcessAsync(new GetRpcResultQuery(eventData.UserId, eventData.ReqMsgId));
        
        if (rpcResultReadModel != null)
        {
            // Found cached result, return it to client
            logger.LogInformation("Returning cached RPC result for userId: {UserId}, reqMsgId: {ReqMsgId}", eventData.UserId, eventData.ReqMsgId);
            var rpcResult = rpcResultReadModel.RpcData.ToTObject<IObject>();
            await messageSender.PushMessageToPeerAsync(eventData.UserId.ToUserPeer(), rpcResult,
                onlySendToThisAuthKeyId: eventData.PermAuthKeyId);
        }
        else
        {
            // Result not yet cached (first command still processing)
            // This is expected for rapid duplicate requests
            logger.LogWarning("Cannot find rpc result for duplicate command, userId: {UserId}, reqMsgId: {ReqMsgId}. First command may still be processing.", eventData.UserId, eventData.ReqMsgId);
            
            // Note: We don't send an error response here because:
            // 1. The first command is likely still being processed
            // 2. The client will receive the response when the first command completes
            // 3. Sending an error could confuse the client or cause UI issues
        }
    }
}