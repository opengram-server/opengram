namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<DeleteMessageSaga2Id>))]
public class DeleteMessageSaga2Id(string value) : SingleValueObject<string>(value), ISagaId;