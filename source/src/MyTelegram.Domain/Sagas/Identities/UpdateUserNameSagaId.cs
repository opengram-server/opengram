namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UpdateUserNameSagaId>))]
public class UpdateUserNameSagaId(string value) : SingleValueObject<string>(value), ISagaId;
