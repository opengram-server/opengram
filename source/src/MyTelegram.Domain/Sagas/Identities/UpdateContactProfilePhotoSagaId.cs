namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UpdateContactProfilePhotoSagaId>))]
public class UpdateContactProfilePhotoSagaId(string value) : SingleValueObject<string>(value), ISagaId;