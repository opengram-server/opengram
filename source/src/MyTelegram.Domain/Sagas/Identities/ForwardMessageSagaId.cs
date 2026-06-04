namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ForwardMessageSagaId>))]
public class ForwardMessageSagaId(string value) : SingleValueObject<string>(value), ISagaId;
