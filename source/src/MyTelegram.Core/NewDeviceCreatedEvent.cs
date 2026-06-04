namespace MyTelegram.Core;

public record NewDeviceCreatedEvent(
    RequestInfo RequestInfo,
    long PermAuthKeyId,
    long TempAuthKeyId,
    long UserId,
    int ApiId,
    string AppName,
    string AppVersion,
    long Hash,
    bool OfficialApp,
    bool PasswordPending,
    string DeviceModel,
    string Platform,
    string SystemVersion,
    string SystemLangCode,
    string LangPack,
    string LangCode,
    string Ip,
    int Layer,
    Dictionary<string, string>? Parameters);