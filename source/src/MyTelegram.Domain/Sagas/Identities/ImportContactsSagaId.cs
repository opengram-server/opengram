namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ImportContactsSagaId>))]
public class ImportContactsSagaId(string value) : SingleValueObject<string>(value), ISagaId;
