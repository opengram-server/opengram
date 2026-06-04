namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<EditChannelTitleSagaId>))]
public class EditChannelTitleSagaId(string value) : SingleValueObject<string>(value), ISagaId;
