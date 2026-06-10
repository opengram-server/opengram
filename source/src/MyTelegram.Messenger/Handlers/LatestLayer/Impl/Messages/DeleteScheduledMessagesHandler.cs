using MyTelegram.Queries.ScheduledMessage;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Delete scheduled messages.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.deleteScheduledMessages" />
///</summary>
internal sealed class DeleteScheduledMessagesHandler(
    IAccessHashHelper accessHashHelper,
    IPeerHelper peerHelper,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    ILogger<DeleteScheduledMessagesHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestDeleteScheduledMessages, MyTelegram.Schema.IUpdates>,
        Messages.IDeleteScheduledMessagesHandler
{
    protected override async Task<IUpdates> HandleCoreAsync(IRequestInput input,
        RequestDeleteScheduledMessages obj)
    {
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);

        // Fetch the scheduled messages
        var scheduledMessages = await queryProcessor.ProcessAsync(
            new GetScheduledMessagesQuery(peer.PeerId, obj.Id.ToList()));

        if (scheduledMessages == null || !scheduledMessages.Any())
        {
            // Nothing to delete — return empty updates
            return new TUpdates
            {
                Updates = new TVector<IUpdate>(),
                Chats = [],
                Users = [],
                Date = CurrentDate
            };
        }

        var updates = new List<IUpdate>();

        foreach (var scheduledMessage in scheduledMessages)
        {
            // Delete the underlying message via the domain command
            var messageId = MessageId.Create(
                scheduledMessage.OwnerPeerId,
                scheduledMessage.ScheduleMessageId);

            var command = new DeleteMessageCommand(messageId, input.ToRequestInfo());
            await commandBus.PublishAsync(command, CancellationToken.None);

            // Notify the client that scheduled message was deleted
            updates.Add(new TUpdateDeleteScheduledMessages
            {
                Peer = peer.ToPeer()!,
                Messages = new TVector<int>(scheduledMessage.ScheduleMessageId)
            });

            logger.LogInformation(
                "Deleted scheduled message {ScheduledMessageId} for peer {PeerId}",
                scheduledMessage.ScheduleMessageId, peer.PeerId);
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
