// ReSharper disable All

using MyTelegram.Queries.ScheduledMessage;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Send scheduled messages right away
/// <para>Possible errors</para>
/// Code Type Description
/// 400 MESSAGE_ID_INVALID The provided message id is invalid.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.sendScheduledMessages" />
///</summary>
internal sealed class SendScheduledMessagesHandler(
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IPeerHelper peerHelper,
    ILogger<SendScheduledMessagesHandler> logger) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSendScheduledMessages, MyTelegram.Schema.IUpdates>,
    Messages.ISendScheduledMessagesHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSendScheduledMessages obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        
        // Get scheduled messages
        var scheduledMessages = await queryProcessor.ProcessAsync(
            new GetScheduledMessagesQuery(peer.PeerId, obj.Id.ToList()));

        if (scheduledMessages == null || !scheduledMessages.Any())
        {
            logger.LogWarning("No scheduled messages found for peer {PeerId}, ids: {Ids}", 
                peer.PeerId, string.Join(",", obj.Id));
            RpcErrors.RpcErrors400.MessageIdInvalid.ThrowRpcError();
        }

        var updates = new List<IUpdate>();
        
        foreach (var scheduledMessage in scheduledMessages)
        {
            // Send updateMessageID to map scheduled message ID to real message ID
            var updateMessageId = new TUpdateMessageID
            {
                Id = scheduledMessage.ActualMessageId ?? scheduledMessage.ScheduleMessageId,
                RandomId = scheduledMessage.RandomId
            };
            updates.Add(updateMessageId);
            
            logger.LogInformation("Sending scheduled message {ScheduledMessageId} -> {ActualMessageId}, RandomId={RandomId}", 
                scheduledMessage.ScheduleMessageId, scheduledMessage.ActualMessageId, scheduledMessage.RandomId);
            
            // TODO: Trigger actual message sending via command
            // This should be handled by a saga or command that converts scheduled message to regular message
        }

        return new TUpdates
        {
            Updates = new TVector<IUpdate>(updates),
            Chats = [],
            Users = [],
            Date = CurrentDate
        };
    }
}
