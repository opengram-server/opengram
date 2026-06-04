namespace MyTelegram.Domain.Aggregates.GroupCall;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<GroupCallId>))]
public class GroupCallId(string value) : Identity<GroupCallId>(value)
{
    public static GroupCallId Create(long callId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"groupcall_{callId}");
    }
}
