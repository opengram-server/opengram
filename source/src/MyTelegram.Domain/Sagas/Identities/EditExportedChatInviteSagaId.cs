namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<EditExportedChatInviteSagaId>))]

public class EditExportedChatInviteSagaId(string value) : SingleValueObject<string>(value), ISagaId;