namespace MyTelegram.Domain.Sagas.Identities;

public class DeleteParticipantHistorySagaId(string value) : SingleValueObject<string>(value), ISagaId;