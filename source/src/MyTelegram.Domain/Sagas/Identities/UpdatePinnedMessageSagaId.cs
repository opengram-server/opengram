namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UpdatePinnedMessageSagaId>))]
public class UpdatePinnedMessageSagaId(string value) : SingleValueObject<string>(value), ISagaId;
