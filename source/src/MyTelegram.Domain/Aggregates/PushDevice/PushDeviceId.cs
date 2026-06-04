namespace MyTelegram.Domain.Aggregates.PushDevice;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<PushDeviceId>))]
public class PushDeviceId(string value) : Identity<PushDeviceId>(value)
{
    public static PushDeviceId Create(string token)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"pushdevice_{token}");
    }
}
