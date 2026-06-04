namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UserSignUpSagaId>))]
public class UserSignUpSagaId(string value) : SingleValueObject<string>(value), ISagaId;
