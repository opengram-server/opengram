namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<JoinChannelSagaId>))]
public class JoinChannelSagaId(string value) : SingleValueObject<string>(value), ISagaId;
