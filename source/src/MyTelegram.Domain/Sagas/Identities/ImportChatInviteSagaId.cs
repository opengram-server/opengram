namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ImportChatInviteSagaId>))]
public class ImportChatInviteSagaId(string value) : SingleValueObject<string>(value), ISagaId;