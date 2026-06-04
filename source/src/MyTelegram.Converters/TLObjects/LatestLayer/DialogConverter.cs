namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class DialogConverter(IObjectMapper objectMapper) : IDialogConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;

    public IDialog ToDialog(IDialogReadModel readModel, IChannelReadModel? channelReadModel,
        IPeerNotifySettings peerNotifySettings)
    {
        var dialog = objectMapper.Map<IDialogReadModel, TDialog>(readModel);
        if (readModel.ReadOutboxMaxId == 0 && readModel.ReadInboxMaxId != 0)
        {
            dialog.ReadInboxMaxId = readModel.ReadInboxMaxId;
        }

        dialog.NotifySettings = peerNotifySettings;

        if (readModel.ToPeerType == PeerType.Channel)
        {
            if (channelReadModel != null)
            {
                var maxId = new[]
                {
                    readModel.MaxSendOutMessageId, readModel.ReadOutboxMaxId,
                    readModel.ReadInboxMaxId,
                    readModel.ChannelHistoryMinId
                }.Max();
                dialog.TopMessage = channelReadModel.TopMessageId;
                dialog.Pts = channelReadModel.Pts;
                dialog.UnreadCount = channelReadModel.TopMessageId - maxId;
                if (dialog.UnreadCount < 0)
                {
                    dialog.UnreadCount = 0;
                }
            }
        }
        else
        {
            dialog.Pts = null;
        }

        return dialog;
    }
}