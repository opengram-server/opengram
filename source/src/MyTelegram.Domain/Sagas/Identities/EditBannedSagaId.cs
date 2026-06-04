namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<EditBannedSagaId>))]
public class EditBannedSagaId(string value) : SingleValueObject<string>(value), ISagaId;