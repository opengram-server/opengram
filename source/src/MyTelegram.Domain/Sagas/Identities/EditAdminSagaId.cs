namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<EditAdminSagaId>))]
public class EditAdminSagaId(string value) : SingleValueObject<string>(value), ISagaId;