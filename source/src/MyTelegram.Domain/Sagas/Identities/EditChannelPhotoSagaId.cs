namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<EditChannelPhotoSagaId>))]
public class EditChannelPhotoSagaId(string value) : SingleValueObject<string>(value), ISagaId;
