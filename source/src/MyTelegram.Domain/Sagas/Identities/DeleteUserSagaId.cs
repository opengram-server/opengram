namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<DeleteUserSagaId>))]
public class DeleteUserSagaId(string value) : SingleValueObject<string>(value), ISagaId;
