namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<CreateUserSagaId>))]
public class CreateUserSagaId(string value) : SingleValueObject<string>(value), ISagaId;