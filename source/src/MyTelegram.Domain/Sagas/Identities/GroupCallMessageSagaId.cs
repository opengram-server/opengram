namespace MyTelegram.Domain.Sagas.Identities;

public class GroupCallMessageSagaId(string value) : SingleValueObject<string>(value), ISagaId;
