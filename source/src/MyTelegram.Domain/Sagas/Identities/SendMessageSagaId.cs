namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<SendMessageSagaId>))]
public class SendMessageSagaId(string value) : SingleValueObject<string>(value), ISagaId;