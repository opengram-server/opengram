namespace MyTelegram.Domain.Aggregates.Contact;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ImportedContactId>))]
public class ImportedContactId(string value) : Identity<ImportedContactId>(value)
{
    public static ImportedContactId Create(long selfUserId,
        string phone)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"importedcontact-{selfUserId}-{phone}");
    }
}
