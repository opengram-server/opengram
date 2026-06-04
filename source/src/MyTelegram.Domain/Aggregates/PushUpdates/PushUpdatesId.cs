namespace MyTelegram.Domain.Aggregates.PushUpdates;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<PushUpdatesId>))]
public class PushUpdatesId(string value) : Identity<PushUpdatesId>(value)
{
    
}
