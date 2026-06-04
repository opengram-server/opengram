namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<MessageSagaId>))]
public class MessageSagaId(string value) : SingleValueObject<string>(value), ISagaId;
