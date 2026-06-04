namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ClearHistorySagaId>))]
public class ClearHistorySagaId(string value) : SingleValueObject<string>(value), ISagaId;
