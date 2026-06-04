namespace MyTelegram.Domain.Commands.Channel;

public class CreateChannelCommand(
    ChannelId aggregateId,
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
    long? migrateFromChatId,
    int? migratedMaxId,
    long? photoId,
    bool autoCreateFromChat = false,
    bool ttlFromDefaultSetting = false,
    List<long>? memberUserIds = null,
    List<long>? botUserIds = null)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public string? About { get; } = about;
    public long AccessHash { get; } = accessHash;
    public string? Address { get; } = address;
    public bool AutoCreateFromChat { get; } = autoCreateFromChat;
    public List<long>? BotUserIds { get; } = botUserIds;
    public bool Broadcast { get; } = broadcast;
    public long ChannelId { get; } = channelId;
    public long CreatorId { get; } = creatorId;
    public int Date { get; } = date;
    public GeoPoint? GeoPoint { get; } = geoPoint;
    public bool MegaGroup { get; } = megaGroup;
    public List<long>? MemberUserIds { get; } = memberUserIds;
    public IMessageAction MessageAction { get; } = messageAction;
    public bool MigratedFromChat { get; } = migratedFromChat;
    public int? MigratedMaxId { get; } = migratedMaxId;
    public long? MigrateFromChatId { get; } = migrateFromChatId;
    public long? PhotoId { get; } = photoId;
    public long RandomId { get; } = randomId;
    public string Title { get; } = title;
    public bool TtlFromDefaultSetting { get; } = ttlFromDefaultSetting;
    public int? TtlPeriod { get; } = ttlPeriod;
}
