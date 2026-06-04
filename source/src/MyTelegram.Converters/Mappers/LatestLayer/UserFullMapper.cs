using System.Diagnostics.CodeAnalysis;

namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class UserFullMapper
    : IObjectMapper<IUserFullReadModel, TUserFull>,
        IObjectMapper<IUserReadModel, TUserFull>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TUserFull Map(IUserFullReadModel source)
    {
        return Map(source, new TUserFull());
    }

    public TUserFull Map(
        IUserFullReadModel source,
        TUserFull destination
    )
    {
        //destination.Blocked = source.Blocked;
        //destination.PhoneCallsAvailable = source.PhoneCallsAvailable;
        //destination.PhoneCallsPrivate = source.PhoneCallsPrivate;
        //destination.CanPinMessage = source.CanPinMessage;
        //destination.HasScheduled = source.HasScheduled;
        //destination.VideoCallsAvailable = source.VideoCallsAvailable;
        //destination.VoiceMessagesForbidden = source.VoiceMessagesForbidden;
        //destination.TranslationsDisabled = source.TranslationsDisabled;
        //destination.StoriesPinnedAvailable = source.StoriesPinnedAvailable;
        //destination.BlockedMyStoriesFrom = source.BlockedMyStoriesFrom;
        //destination.WallpaperOverridden = source.WallpaperOverridden;
        //destination.ContactRequirePremium = source.ContactRequirePremium;
        //destination.ReadDatesPrivate = source.ReadDatesPrivate;
        //destination.SponsoredEnabled = source.SponsoredEnabled;
        //destination.CanViewRevenue = source.CanViewRevenue;
        //destination.BotCanManageEmojiStatus = source.BotCanManageEmojiStatus;
        //destination.Id = source.Id;
        //destination.About = source.About;
        //destination.Settings = source.Settings;
        //destination.PersonalPhoto = source.PersonalPhoto;
        //destination.ProfilePhoto = source.ProfilePhoto;
        //destination.FallbackPhoto = source.FallbackPhoto;
        //destination.NotifySettings = source.NotifySettings;
        //destination.BotInfo = source.BotInfo;
        //destination.PinnedMsgId = source.PinnedMsgId;
        //destination.CommonChatsCount = source.CommonChatsCount;
        //destination.FolderId = source.FolderId;
        //destination.TtlPeriod = source.TtlPeriod;
        //destination.ThemeEmoticon = source.ThemeEmoticon;
        //destination.PrivateForwardName = source.PrivateForwardName;
        //destination.BotGroupAdminRights = source.BotGroupAdminRights;
        //destination.BotBroadcastAdminRights = source.BotBroadcastAdminRights;
        //destination.PremiumGifts = source.PremiumGifts;
        //destination.Wallpaper = source.Wallpaper;
        //destination.Stories = source.Stories;
        destination.BusinessWorkHours = source.BusinessWorkHours;
        destination.BusinessLocation = source.BusinessLocation;
        destination.BusinessGreetingMessage = source.BusinessGreetingMessage;
        destination.BusinessAwayMessage = source.BusinessAwayMessage;
        destination.BusinessIntro = source.BusinessIntro;
        //destination.Birthday = source.Birthday;
        destination.PersonalChannelId = source.PersonalChannelId;
        destination.PersonalChannelMessage = source.PersonalChannelMessage;
        //destination.StargiftsCount = source.StargiftsCount;
        //destination.StarrefProgram = source.StarrefProgram;
        //destination.BotVerification = source.BotVerification;

        destination.Id = source.UserId;
        destination.Settings = new TPeerSettings();

        // Включаем кнопку звёздных подарков для всех пользователей
        destination.DisplayGiftsButton = true;

        return destination;
    }

    [return: NotNullIfNotNull("source")]
    public TUserFull? Map(IUserReadModel source)
    {
        return Map(source, new TUserFull());
    }

    [return: NotNullIfNotNull("source")]
    public TUserFull? Map(IUserReadModel source, TUserFull destination)
    {
        destination.Id = source.UserId;
        destination.About = source.About;
        destination.Settings = new TPeerSettings();
        destination.ReadDatesPrivate = source.GlobalPrivacySettings?.HideReadMarks ?? false;

        // Включаем кнопку звёздных подарков для всех пользователей
        destination.DisplayGiftsButton = true;

        // Маппинг бизнес-полей (TODO: нужен конвертер из Domain.Shared в типы Schema)
        // destination.BusinessWorkHours = source.BusinessWorkHours;
        // destination.BusinessLocation = source.BusinessLocation;
        // destination.BusinessGreetingMessage = source.BusinessGreetingMessage;
        // destination.BusinessAwayMessage = source.BusinessAwayMessage;
        // destination.BusinessIntro = source.BusinessIntro;
        
        if (source.Birthday != null)
        {
            destination.Birthday = new TBirthday
            {
                Day = source.Birthday.Day,
                Month = source.Birthday.Month,
                Year = source.Birthday.Year
            };
        }

        return destination;
    }
}