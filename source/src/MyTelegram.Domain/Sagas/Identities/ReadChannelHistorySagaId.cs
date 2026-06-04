namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ReadChannelHistorySagaId>))]
public class ReadChannelHistorySagaId(string value) : SingleValueObject<string>(value), ISagaId;
