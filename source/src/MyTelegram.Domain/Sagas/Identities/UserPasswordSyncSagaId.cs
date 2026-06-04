namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UserPasswordSyncSagaId>))]
public class UserPasswordSyncSagaId(string value) : SingleValueObject<string>(value), ISagaId;
