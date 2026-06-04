namespace MyTelegram.Domain.Aggregates.PhoneCall;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<PhoneCallId>))]
public class PhoneCallId(string value) : Identity<PhoneCallId>(value)
{
    public static PhoneCallId Create(long callId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"phonecall_{callId}");
    }
}
