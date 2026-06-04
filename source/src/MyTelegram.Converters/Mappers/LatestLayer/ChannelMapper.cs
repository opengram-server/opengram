namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class ChannelMapper
    : IObjectMapper<IChannelReadModel, TChannel>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TChannel Map(IChannelReadModel source)
    {
        return Map(source, new TChannel());
    }

    public TChannel Map(
        IChannelReadModel source,
        TChannel destination
    )
    {
        destination.Id = source.ChannelId;
        destination.Title = source.Title;
        destination.ParticipantsCount = source.ParticipantsCount;
        destination.Broadcast = source.Broadcast;
        destination.Megagroup = source.MegaGroup;
        destination.Verified = source.Verified;
        destination.Signatures = source.Signatures;
        destination.AccessHash = source.AccessHash;
        destination.Username = source.UserName;
        destination.Date = source.Date;
        destination.SlowmodeEnabled = source.SlowModeEnabled;

        destination.DefaultBannedRights = (source.DefaultBannedRights ?? ChatBannedRights.CreateDefaultBannedRights())
            .ToChatBannedRights();

        destination.HasLink = source.HasLink;
        destination.Noforwards = source.NoForwards;
        destination.Color = source.Color.ToPeerColor();
        destination.ProfileColor = source.ProfileColor.ToPeerColor();
        destination.Forum = source.Forum;

        if (source.BackgroundEmojiId.HasValue)
        {
            destination.EmojiStatus = new TEmojiStatus
            {
                DocumentId = source.BackgroundEmojiId.Value
            };
        }

        destination.Level = 10;

        if (source.EmojiStatus != null)
        {
            if (source.EmojiStatus.Until.HasValue)
            {
                //destination.EmojiStatus = new TEmojiStatusUntil
                //{
                //    DocumentId = source.EmojiStatus.DocumentId,
                //    Until = source.EmojiStatus.Until.Value
                //};
                //destination.EmojiStatus = new TEmojiStatusCollectible
                //{
                //    Until = source.EmojiStatus.Until,
                //    DocumentId=source.EmojiStatus.DocumentId
                //};
            }
            else
            {
                destination.EmojiStatus = new TEmojiStatus
                {
                    DocumentId = source.EmojiStatus.DocumentId
                };
            }
        }

        destination.SignatureProfiles = source.SignatureProfiles;
        destination.SubscriptionUntilDate = source.SubscriptionUntilDate;
        if (source.Usernames?.Count > 0)
        {
            destination.Usernames = [];
            if (!string.IsNullOrEmpty(destination.Username))
            {
                destination.Usernames.Add(new TUsername
                {
                    Active = true,
                    Editable = true,
                    Username = destination.Username
                });
                destination.Username = null;
            }

            foreach (var username in source.Usernames)
            {
                destination.Usernames.Add(new TUsername
                {
                    Active = true,
                    Editable = false,
                    Username = username
                });
            }
        }

        destination.JoinRequest = source.JoinRequest;
        destination.JoinToSend = source.JoinToSend;
        destination.SendPaidMessagesStars = source.SendPaidMessagesStars;
        destination.BroadcastMessagesAllowed = source.BroadcastMessagesAllowed;
        destination.LinkedMonoforumId = source.LinkedMonoforumId;
        destination.Monoforum = source.Monoforum;

        return destination;
    }
}