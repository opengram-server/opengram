namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<SignInSagaId>))]
public class SignInSagaId(string value) : SingleValueObject<string>(value), ISagaId;