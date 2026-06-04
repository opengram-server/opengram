namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<InviteToChannelSagaId>))]
public class InviteToChannelSagaId(string value) : SingleValueObject<string>(value), ISagaId;
