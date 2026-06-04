namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ApproveJoinChannelSagaId>))]
public class ApproveJoinChannelSagaId(string value) : SingleValueObject<string>(value), ISagaId;