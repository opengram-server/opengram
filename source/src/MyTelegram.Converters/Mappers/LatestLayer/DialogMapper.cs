namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class DialogMapper
    : IObjectMapper<IDialogReadModel, TDialog>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TDialog Map(IDialogReadModel source)
    {
        return Map(source, new TDialog());
    }

    public TDialog Map(
        IDialogReadModel source,
        TDialog destination
    )
    {
        destination.Pts = source.Pts;
        destination.TopMessage = source.TopMessage;
        destination.Pinned = source.Pinned;
        destination.UnreadCount = source.UnreadCount;
        //destination.UnreadMark = source.u;
        destination.ReadInboxMaxId = source.ReadInboxMaxId;
        destination.ReadOutboxMaxId = source.ReadOutboxMaxId;
        destination.Peer = new Peer(source.ToPeerType, source.ToPeerId).ToPeer();
        //if (source.Draft?.Message?.Length > 0)
        //{
        //    destination.Draft = new TDraftMessage
        //    {
        //        Date = source.Draft.Date,
        //        Message = source.Draft.Message,
        //        NoWebpage = source.Draft.NoWebpage,
        //        ReplyTo = new TInputReplyToMessage
        //        {
        //            ReplyToMsgId = source.Draft.ReplyToMsgId ?? 0
        //        },
        //        Entities = source.Draft.Entities.ToTObject<TVector<IMessageEntity>>(),
        //        InvertMedia = source.Draft.InvertMedia,
        //        Effect = source.Draft.Effect
        //    };
        //}

        //destination.NotifySettings = new TPeerNotifySettings
        //{
        //    ShowPreviews = true,
        //    Silent = false,
        //    //Sound = "default",
        //    MuteUntil = 0
        //};
        if (source.NotifySettings != null)
        {
            destination.NotifySettings = new TPeerNotifySettings
            {
                AndroidSound = new TNotificationSoundDefault(),
                IosSound = new TNotificationSoundDefault(),
                MuteUntil = source.NotifySettings.MuteUntil,
                OtherSound = new TNotificationSoundDefault(),
                ShowPreviews = source.NotifySettings.ShowPreviews,
                Silent = source.NotifySettings.Silent
            };
        }
        else
        {
            destination.NotifySettings = new TPeerNotifySettings();
        }

        destination.TtlPeriod = source.TtlPeriod;
        destination.UnreadMentionsCount = source.UnreadMentionsCount;
        destination.UnreadReactionsCount = source.UnreadReactionsCount;
        destination.FolderId = source.FolderId;
        destination.ViewForumAsMessages = source.ViewForumAsMessages;

        return destination;
    }
}