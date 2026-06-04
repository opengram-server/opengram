using EventFlow.Aggregates;

namespace MyTelegram.Domain.Aggregates.Device;

public class DeviceState : AggregateState<DeviceAggregate, DeviceId, DeviceState>,
    IApply<DeviceCreatedEvent>,
    IApply<BindUidToDeviceEvent>,
    IApply<DeviceAuthKeyUnRegisteredEvent>
{
    public long PermAuthKeyId { get; private set; }
    public long TempAuthKeyId { get; private set; }
    public long UserId { get; private set; }
    public Dictionary<string, string>? Parameters { get; private set; } = [];
    public int ApiId { get; private set; }
    public string AppName { get; private set; } = null!;
    public string AppVersion { get; private set; } = null!;
    public long Hash { get; private set; }
    public bool OfficialApp { get; private set; }
    public bool PasswordPending { get; private set; }
    public string DeviceModel { get; private set; } = null!;
    public string Platform { get; private set; } = null!;
    public string SystemVersion { get; private set; } = null!;
    public string SystemLangCode { get; private set; } = null!;
    public string LangPack { get; private set; } = null!;
    public string LangCode { get; private set; } = null!;
    public string Ip { get; private set; } = null!;
    public int Layer { get; private set; }
    public int DateCreated { get; private set; }
    public int DateActive { get; private set; }
    public bool IsActive { get; private set; }
    public void Apply(BindUidToDeviceEvent aggregateEvent)
    {
        UserId = aggregateEvent.UserId;
    }

    public void Apply(DeviceAuthKeyUnRegisteredEvent aggregateEvent)
    {
    }

    public void LoadSnapshot(DeviceSnapshot snapshot)
    {
        PermAuthKeyId = snapshot.PermAuthKeyId;
        TempAuthKeyId = snapshot.TempAuthKeyId;
        UserId = snapshot.UserId;
        ApiId = snapshot.ApiId;
        AppName = snapshot.AppName;
        AppVersion = snapshot.AppVersion;
        OfficialApp = snapshot.OfficialApp;
        PasswordPending = snapshot.PasswordPending;
        DeviceModel = snapshot.DeviceModel;
        Platform = snapshot.Platform;
        SystemVersion = snapshot.SystemVersion;
        SystemLangCode = snapshot.SystemLangCode;
        LangPack = snapshot.LangPack;
        LangCode = snapshot.LangCode;
        Ip = snapshot.Ip;
        Layer = snapshot.Layer;
        DateCreated = snapshot.Date;
        DateActive = snapshot.Date;
        IsActive = snapshot.IsActive;
        Hash = snapshot.Hash;
        Parameters = snapshot.Parameters;
    }

    public void Apply(DeviceCreatedEvent aggregateEvent)
    {
        PermAuthKeyId = aggregateEvent.PermAuthKeyId;
        TempAuthKeyId= aggregateEvent.TempAuthKeyId;
        UserId = aggregateEvent.UserId;
        ApiId = aggregateEvent.ApiId;
        AppName = aggregateEvent.AppName;
        AppVersion = aggregateEvent.AppVersion;
        OfficialApp = aggregateEvent.OfficialApp;
        PasswordPending = aggregateEvent.PasswordPending;
        DeviceModel = aggregateEvent.DeviceModel;
        Platform = aggregateEvent.Platform;
        SystemVersion = aggregateEvent.SystemVersion;
        SystemLangCode = aggregateEvent.SystemLangCode;
        LangPack = aggregateEvent.LangPack;
        LangCode = aggregateEvent.LangCode;
        Ip = aggregateEvent.Ip;
        Layer = aggregateEvent.Layer;
        DateCreated = aggregateEvent.Date;
        DateActive = aggregateEvent.Date;
        IsActive = true;
        Hash = aggregateEvent.Hash;
        Parameters = aggregateEvent.Parameters;
    }
}
