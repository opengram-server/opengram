namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Delete the entire phone call history.
/// See <a href="https://corefork.telegram.org/method/messages.deletePhoneCallHistory" />
///</summary>
internal sealed class DeletePhoneCallHistoryHandler(
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IPtsHelper ptsHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestDeletePhoneCallHistory, MyTelegram.Schema.Messages.IAffectedFoundMessages>,
    Messages.IDeletePhoneCallHistoryHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IAffectedFoundMessages> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestDeletePhoneCallHistory obj)
    {
        // Query messages with PhoneCall action type owned by the requesting user
        var phoneCallMessages = await queryProcessor.ProcessAsync(
            new GetMessagesQuery(
                OwnerPeerId: input.UserId,
                MessageType: MessageType.Unknown,
                Q: null,
                MessageIdList: null,
                ChannelHistoryMinId: 0,
                Limit: 100,
                Offset: null,
                Peer: null,
                SelfUserId: input.UserId,
                Pts: 0,
                MessageActionType: MessageActionType.PhoneCall),
            CancellationToken.None);

        if (phoneCallMessages.Count == 0)
        {
            return new MyTelegram.Schema.Messages.TAffectedFoundMessages
            {
                Pts = ptsHelper.GetCachedPts(input.UserId),
                PtsCount = 0,
                Offset = 0,
                Messages = []
            };
        }

        var messageIds = phoneCallMessages.Select(m => m.MessageId).ToList();

        // Use the same deletion pipeline as messages.deleteMessages
        var messageItemsToBeDeleted = await queryProcessor.ProcessAsync(
            new GetMessageItemListToBeDeletedQuery(input.UserId, messageIds, obj.Revoke),
            CancellationToken.None);

        int? newTopMessageId = null;
        int? newTopMessageIdForOtherParticipant = null;

        if (messageItemsToBeDeleted.Any(p => p.ToPeerType == PeerType.User))
        {
            newTopMessageId = await queryProcessor.ProcessAsync(
                new GetTopMessageIdQuery(input.UserId, messageIds),
                CancellationToken.None);

            if (obj.Revoke)
            {
                var toPeerMessageItem = messageItemsToBeDeleted
                    .FirstOrDefault(p => p.OwnerUserId != input.UserId);
                if (toPeerMessageItem != null)
                {
                    var toPeerMessageIds = messageItemsToBeDeleted
                        .Where(p => p.OwnerUserId != input.UserId)
                        .Select(p => p.MessageId).ToList();
                    newTopMessageIdForOtherParticipant = await queryProcessor.ProcessAsync(
                        new GetTopMessageIdQuery(toPeerMessageItem.OwnerUserId, toPeerMessageIds),
                        CancellationToken.None);
                }
            }
        }

        var command = new StartDeleteMessagesCommand(
            TempId.New,
            input.ToRequestInfo(),
            messageItemsToBeDeleted,
            obj.Revoke,
            false, // deleteGroupMessagesForEveryone
            newTopMessageId,
            newTopMessageIdForOtherParticipant);

        await commandBus.PublishAsync(command, CancellationToken.None);

        return new MyTelegram.Schema.Messages.TAffectedFoundMessages
        {
            Pts = ptsHelper.GetCachedPts(input.UserId),
            PtsCount = messageIds.Count,
            Offset = 0,
            Messages = new TVector<int>(messageIds)
        };
    }
}
