namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Mark mentions as read. Per corefork: marks all mentions in a dialog as read,
/// optionally filtered by top_msg_id for forum topics.
/// See <a href="https://corefork.telegram.org/method/messages.readMentions" />
///</summary>
internal sealed class ReadMentionsHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IPtsHelper ptsHelper,
    IPeerHelper peerHelper,
    IAccessHashHelper accessHashHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestReadMentions, MyTelegram.Schema.Messages.IAffectedHistory>,
    Messages.IReadMentionsHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IAffectedHistory> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestReadMentions obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);

        // Get current dialog to know how many unread mentions exist
        var dialogId = DialogId.Create(input.UserId, peer);
        var dialog = await queryProcessor.ProcessAsync(
            new GetDialogByIdQuery(dialogId.Value),
            CancellationToken.None);

        if (dialog == null || dialog.UnreadMentionsCount == 0)
        {
            return new MyTelegram.Schema.Messages.TAffectedHistory
            {
                Pts = ptsHelper.GetCachedPts(input.UserId),
                PtsCount = 0,
                Offset = 0
            };
        }

        // Query all unread mentioned message IDs for this dialog
        // top_msg_id filtering: if provided, only clear mentions within that topic/thread
        var messageIds = await queryProcessor.ProcessAsync(
            new GetUnreadMentionedMessageIdListQuery(
                input.UserId,
                peer.PeerId,
                null,
                0,
                dialog.UnreadMentionsCount + 10), // fetch all
            CancellationToken.None);

        // If top_msg_id is specified, filter to only mentions in that thread/topic
        var filteredIds = messageIds;
        if (obj.TopMsgId.HasValue && obj.TopMsgId.Value > 0)
        {
            // Load messages to check TopMsgId
            var msgIdStrings = messageIds.Select(id =>
                MessageId.Create(input.UserId, id).Value).ToList();
            if (msgIdStrings.Count > 0)
            {
                var messages = await queryProcessor.ProcessAsync(
                    new GetMessagesByIdListQuery(msgIdStrings),
                    CancellationToken.None);
                filteredIds = messages
                    .Where(m => m.TopMsgId == obj.TopMsgId.Value || m.ReplyToMsgId == obj.TopMsgId.Value)
                    .Select(m => (int)m.SenderMessageId) // SenderMessageId as per IMessageReadModel interface
                    .ToList();
            }
        }

        // Issue ReadMention command for each mention to decrement the counter
        var readCount = 0;
        foreach (var messageId in filteredIds)
        {
            try
            {
                var command = new ReadMentionCommand(dialogId, input.UserId, messageId);
                await commandBus.PublishAsync(command, CancellationToken.None);
                readCount++;
            }
            catch
            {
                // Continue even if one fails — aggregate might not exist
            }
        }

        return new MyTelegram.Schema.Messages.TAffectedHistory
        {
            Pts = ptsHelper.GetCachedPts(input.UserId),
            PtsCount = readCount,
            Offset = 0 // 0 means all mentions have been processed
        };
    }
}
