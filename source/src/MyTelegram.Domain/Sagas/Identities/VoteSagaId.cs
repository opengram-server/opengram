namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<VoteSagaId>))]
public class VoteSagaId(string value) : SingleValueObject<string>(value), ISagaId;