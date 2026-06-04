namespace MyTelegram.Domain.Aggregates.Device;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<DeviceId>))]
public class DeviceId(string value) : Identity<DeviceId>(value)
{
    public static DeviceId Create(long permAuthKeyId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"device_{permAuthKeyId}");
    }
}
