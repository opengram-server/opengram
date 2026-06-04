namespace MyTelegram.Messenger.Converters.ConverterServices.Messages;

internal sealed class EditMessageConverterService(IMessageConverterService messageConverterService)
    : IEditMessageConverterService, ITransientDependency
{
    public IUpdates ToEditMessageUpdates(InboxMessageEditCompletedSagaEvent data)
    {
        var update = new TUpdateEditMessage
        {
            Message = messageConverterService.ToMessage(0, data.NewMessageItem),
            Pts = data.NewMessageItem.Pts,
            PtsCount = 1
        };

        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = [],
            Chats = [],
            Date = DateTime.UtcNow.ToTimestamp(),
            Seq = 0
        };
    }

    public IUpdates ToEditMessageUpdates(long selfUserId, OutboxMessageEditCompletedSagaEvent data, int layer)
    {
        return ToEditMessageUpdatesCore(selfUserId, data, layer);
    }

    private IUpdates ToEditMessageUpdatesCore(long selfUserId, OutboxMessageEditCompletedSagaEvent data, int layer)
    {
        IUpdate update = data.NewMessageItem.ToPeer.PeerType switch
        {
            PeerType.Channel => new TUpdateEditChannelMessage
            {
                Pts = data.NewMessageItem.Pts,
                PtsCount = 1,
                Message = messageConverterService.ToMessage(selfUserId, data.NewMessageItem, layer: layer)
            },
            _ => new TUpdateEditMessage
            {
                Message = messageConverterService.ToMessage(data.NewMessageItem.SenderPeer.PeerId, data.NewMessageItem, layer: layer),
                Pts = data.NewMessageItem.Pts,
                PtsCount = 1
            }
        };

        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = [],
            Chats = [],
            Date = DateTime.UtcNow.ToTimestamp(),
            Seq = 0
        };
    }

    public IUpdates ToEditQuickReplyMessageUpdates(OutboxMessageEditCompletedSagaEvent data, int layer)
    {
        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(new TUpdateQuickReplyMessage
            {
                Message = messageConverterService.ToMessage(data.RequestInfo.UserId, data.NewMessageItem, layer: layer)
            }),
            Users = [],
            Chats = [],
            Date = DateTime.UtcNow.ToTimestamp()
        };

        return updates;
    }
}