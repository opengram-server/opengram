namespace MyTelegram.Domain.Aggregates.Contact;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ContactId>))]
public class ContactId(string value) : Identity<ContactId>(value)
{
    public static ContactId Create(long selfUserId,
        long targetUserId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"contact-{selfUserId}-{targetUserId}");
    }
}
