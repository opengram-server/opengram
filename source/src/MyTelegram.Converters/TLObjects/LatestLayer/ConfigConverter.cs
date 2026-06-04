namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class ConfigConverter(IObjectMapper objectMapper) : IConfigConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IConfig ToConfig(List<DcOption> dcOptions,
        int thisDcId,
        int mediaDcId)
    {
        var options = dcOptions.Select(p => objectMapper.Map<DcOption, TDcOption>(p));

        var date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expires = (int)DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds();
        
        var config = new TConfig
        {
            //Flags = new System.Collections.BitArray(new byte[] { 0 }),
            //PhonecallsEnabled = true,
            DefaultP2pContacts = true,
            //PreloadFeaturedStickers = true,
            //IgnorePhoneEntities = false,
            RevokePmInbox = true,
            BlockedMode = false,
            //PfsEnabled = true,
            Date = date,
            // на android — 30 минут
            Expires = expires,

            TestMode = false,
            ThisDc = thisDcId,
            ForceTryIpv6 = true,
            //DcOptions = new TVector<IDcOption>(),
            DcOptions = new TVector<IDcOption>(options),
            DcTxtDomainName = "test.com",
            ChatSizeMax = 200, // _options.ChatSizeMax, //200
            MegagroupSizeMax = 200000, // _options.MegagroupSizeMax, //200000
            ForwardedCountMax = 100, // _options.ForwardedCountMax, //100
            OnlineUpdatePeriodMs = 210000,
            OfflineBlurTimeoutMs = 5000,
            OfflineIdleTimeoutMs = 30000,
            OnlineCloudTimeoutMs = 300000,
            NotifyCloudDelayMs = 30000,
            NotifyDefaultDelayMs = 1500,
            PushChatPeriodMs = 60000,
            PushChatLimit = 2,
            //SavedGifsLimit = 200,
            EditTimeLimit = 172800, // _options.EditTimeLimit, //172800
            RevokeTimeLimit = 2147483647,
            RevokePmTimeLimit = 2147483647,
            RatingEDecay = 2419200,
            StickersRecentLimit = 200,
            //StickersFavedLimit = 5,
            ChannelsReadMediaPeriod = 604800,
            //TmpSessions =
            //PinnedDialogsCountMax = 5, // _options.PinnedDialogsCountMax, //5
            //PinnedInfolderCountMax = 100,
            CallReceiveTimeoutMs = 20000,
            CallRingTimeoutMs = 90000,
            CallConnectTimeoutMs = 30000,
            CallPacketTimeoutMs = 10000,
            MeUrlPrefix = "https://t.me/",
            //AutoupdateUrlPrefix = "",
            GifSearchUsername = "gif",
            VenueSearchUsername = "foursquare",
            ImgSearchUsername = "bing",
            StaticMapsProvider = "telegram,google:AIzaSyB0y3zA4LbA04ZPaHKsr_Xt5ZQWbMftj8I",
            CaptionLengthMax = 1024, // _options.CaptionLengthMax, //1024
            MessageLengthMax = 4096, // _options.MessageLengthMax, //4096
            WebfileDcId = mediaDcId, // _dataCenterHelper.GetMediaDcId(),
            SuggestedLangCode = "en",
            LangPackVersion = 0,
            BaseLangPackVersion = 0,
            ReactionsDefault = new TReactionEmoji
            {
                Emoticon = "👍"
            },
            StargiftsEnabled = true
        };

        return config;
    }
}