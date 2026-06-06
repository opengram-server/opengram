namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get unread mentions.
/// See <a href="https://corefork.telegram.org/method/messages.getUnreadMentions" />
///</summary>
internal sealed class GetUnreadMentionsHandler(
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper,
    IAccessHashHelper accessHashHelper,
    IMessageConverterService messageConverterService,
    IUserAppService userAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetUnreadMentions, MyTelegram.Schema.Messages.IMessages>,
    Messages.IGetUnreadMentionsHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IMessages> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetUnreadMentions obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        await accessHashHelper.CheckAccessHashAsync(input, obj.Peer);

        // Build offset for pagination
        OffsetInfo? offset = null;
        if (obj.OffsetId > 0)
        {
            offset = new OffsetInfo { MaxId = obj.OffsetId };
        }

        var limit = Math.Min(obj.Limit, 100);
        var skip = obj.AddOffset > 0 ? obj.AddOffset : 0;

        // Query unread mentioned message IDs via MongoDB
        var messageIds = await queryProcessor.ProcessAsync(
            new GetUnreadMentionedMessageIdListQuery(
                input.UserId,
                peer.PeerId,
                offset,
                skip,
                limit),
            CancellationToken.None);

        if (messageIds.Count == 0)
        {
            return new MyTelegram.Schema.Messages.TMessages
            {
                Messages = [],
                Chats = [],
                Users = []
            };
        }

        // Load full message read models
        var messageIdStrings = messageIds.Select(id =>
            MessageId.Create(input.UserId, id).Value).ToList();
        var messages = await queryProcessor.ProcessAsync(
            new GetMessagesByIdListQuery(messageIdStrings),
            CancellationToken.None);

        // Convert to TL objects using the converter service
        var tlMessages = messages
            .Select(m => messageConverterService.ToMessage(input.UserId, m, layer: input.Layer))
            .ToList();

        // Gather user IDs for the Users field
        var userIds = messages
            .Select(m => m.SenderUserId)
            .Concat(messages.Where(m => m.ToPeerType == PeerType.User).Select(m => m.ToPeerId))
            .Where(id => id > 0)
            .Distinct()
            .ToList();
        var users = await userAppService.GetListAsync(userIds);

        var tlUsers = users.Select(u =>
        {
            IUser tUser = new TUser
            {
                Id = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Phone = u.PhoneNumber,
                AccessHash = u.AccessHash,
                Bot = u.Bot,
                Premium = u.Premium
            };
            return tUser;
        }).ToList();

        // Get dialog for total unread mentions count
        var dialogId = DialogId.Create(input.UserId, peer);
        var dialog = await queryProcessor.ProcessAsync(
            new GetDialogByIdQuery(dialogId.Value),
            CancellationToken.None);

        var totalCount = dialog?.UnreadMentionsCount ?? messageIds.Count;

        return new MyTelegram.Schema.Messages.TMessagesSlice
        {
            Count = totalCount,
            Messages = new TVector<Schema.IMessage>(tlMessages),
            Chats = [],
            Users = new TVector<Schema.IUser>(tlUsers)
        };
    }
}
