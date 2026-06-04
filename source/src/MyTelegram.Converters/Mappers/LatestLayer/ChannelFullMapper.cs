namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class ChannelFullMapper
    : IObjectMapper<IChannelFullReadModel, TChannelFull>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;


    public TChannelFull Map(IChannelFullReadModel source)
    {
        return Map(source, new TChannelFull());
    }

    public TChannelFull Map(
        IChannelFullReadModel source,
        TChannelFull destination
    )
    {
        destination.Id = source.ChannelId;
        destination.About = source.About ?? string.Empty;

        destination.CanViewParticipants = source.CanViewParticipants;
        destination.CanSetUsername = source.CanSetUserName;
        destination.CanSetStickers = source.CanSetStickers;
        destination.HiddenPrehistory = source.HiddenPreHistory;
        destination.CanViewStats = source.CanViewStats;
        destination.CanSetLocation = source.CanSetLocation;
        destination.AdminsCount = source.AdminsCount;
        destination.KickedCount = source.KickedCount;
        destination.BannedCount = source.BannedCount;
        destination.OnlineCount = source.OnlineCount;
        destination.ReadInboxMaxId = source.ReadInboxMaxId;
        destination.ReadOutboxMaxId = source.ReadOutboxMaxId;
        destination.UnreadCount = source.UnreadCount;
        destination.MigratedFromChatId = source.MigratedFromChatId;
        destination.MigratedFromMaxId = source.MigratedFromMaxId;
        destination.PinnedMsgId = source.PinnedMsgId;
        destination.AvailableMinId = source.AvailableMinId;
        destination.FolderId = source.FolderId;
        destination.LinkedChatId = source.LinkedChatId;
        destination.SlowmodeSeconds = source.SlowModeSeconds;
        destination.SlowmodeNextSendDate = source.SlowModeNextSendDate;
        switch (source.ReactionType)
        {
            case ReactionType.ReactionNone:
                // Enable all reactions by default
                destination.AvailableReactions = new TChatReactionsAll
                {
                    AllowCustom = true
                };
                break;
            case ReactionType.ReactionAll:
                destination.AvailableReactions = new TChatReactionsAll
                {
                    AllowCustom = source.AllowCustomReaction
                };
                break;
            case ReactionType.ReactionSome:
                if (source.AvailableReactions?.Count > 0)
                {
                    destination.AvailableReactions = new TChatReactionsSome
                    {
                        Reactions = new TVector<IReaction>(source.AvailableReactions.Select(p => new TReactionEmoji
                        {
                            Emoticon = p
                        }))
                    };
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        destination.Antispam = source.AntiSpam;
        destination.TranslationsDisabled = false;
        if (source.TtlPeriod != 0)
        {
            destination.TtlPeriod = source.TtlPeriod;
        }

        destination.ParticipantsHidden = source.ParticipantsHidden;
        // Enable Star Gifts for all channels/users
        destination.StargiftsAvailable = true;
        destination.SendPaidMessagesStars = source.SendPaidMessagesStars;
        destination.PaidMessagesAvailable = source.PaidMessagesAvailable;
        return destination;
    }
}