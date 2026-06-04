namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ReadHistorySagaId>))]
public class ReadHistorySagaId(string value) : SingleValueObject<string>(value), ISagaId;
