namespace MyTelegram.Domain.Events.Channel;

public class ChannelCreatedEvent(
    RequestInfo requestInfo,
    long channelId,
    long creatorId,
    string title,
    bool broadcast,
    bool megaGroup,
    string? about,
    GeoPoint? geoPoint,
    string? address,
    long accessHash,
    int date,
    long randomId,
    IMessageAction messageAction,
    int? ttlPeriod,
    bool migratedFromChat,
    long? migratedFromChatId,
    int? migratedMaxId,
    long? photoId,
    bool autoCreateFromChat,
    bool ttlFromDefaultSetting,
    List<long> memberUserIds,
    List<long> botUserIds)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public string? About { get; } = about;
    public GeoPoint? GeoPoint { get; } = geoPoint;
    public long AccessHash { get; } = accessHash;
    public string? Address { get; } = address;

    public bool Broadcast { get; } = broadcast;
    public long ChannelId { get; } = channelId;
    public long CreatorId { get; } = creatorId;
    public int Date { get; } = date;
    public bool MegaGroup { get; } = megaGroup;
    public int? TtlPeriod { get; } = ttlPeriod;
    public bool MigratedFromChat { get; } = migratedFromChat;
    public long? MigratedFromChatId { get; } = migratedFromChatId;
    public int? MigratedMaxId { get; } = migratedMaxId;
    public long? PhotoId { get; } = photoId;
    public bool AutoCreateFromChat { get; } = autoCreateFromChat;
    public bool TtlFromDefaultSetting { get; } = ttlFromDefaultSetting;
    public List<long> MemberUserIds { get; } = memberUserIds;
    public List<long> BotUserIds { get; } = botUserIds;
    public long RandomId { get; } = randomId;
    public IMessageAction MessageAction { get; } = messageAction;
    public string Title { get; } = title;
}