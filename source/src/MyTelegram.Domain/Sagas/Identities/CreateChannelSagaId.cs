namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<CreateChannelSagaId>))]
public class CreateChannelSagaId(string value) : SingleValueObject<string>(value), ISagaId;
