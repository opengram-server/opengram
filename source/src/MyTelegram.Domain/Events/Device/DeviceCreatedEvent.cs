namespace MyTelegram.Domain.Events.Device;

public class DeviceCreatedEvent(
    bool isNewDevice,
    long permAuthKeyId,
    long tempAuthKeyId,
    long userId,
    int apiId,
    string appName,
    string appVersion,
    long hash,
    bool officialApp,
    bool passwordPending,
    string deviceModel,
    string platform,
    string systemVersion,
    string systemLangCode,
    string langPack,
    string langCode,
    string ip,
    int layer,
    int date,
    Dictionary<string,string>? parameters
    )
    : AggregateEvent<DeviceAggregate, DeviceId>
{
    public int ApiId { get; } = apiId;
    public string AppName { get; } = appName;
    public string AppVersion { get; } = appVersion;
    public int Date { get; } = date;
    public Dictionary<string, string>? Parameters { get; } = parameters;
    public string DeviceModel { get; } = deviceModel;
    public long Hash { get; } = hash;
    public string Ip { get; } = ip;
    public bool IsNewDevice { get; } = isNewDevice;
    public string LangCode { get; } = langCode;
    public string LangPack { get; } = langPack;
    public int Layer { get; } = layer;
    public bool OfficialApp { get; } = officialApp;
    public bool PasswordPending { get; } = passwordPending;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public string Platform { get; } = platform;
    public string SystemLangCode { get; } = systemLangCode;
    public string SystemVersion { get; } = systemVersion;
    public long TempAuthKeyId { get; } = tempAuthKeyId;
    public long UserId { get; } = userId;
}
