namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<DeleteMessageSagaId>))]
public class DeleteMessageSagaId(string value) : SingleValueObject<string>(value), ISagaId;