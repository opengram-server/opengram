namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<EditMessageSagaId>))]
public class EditMessageSagaId(string value) : SingleValueObject<string>(value), ISagaId;
