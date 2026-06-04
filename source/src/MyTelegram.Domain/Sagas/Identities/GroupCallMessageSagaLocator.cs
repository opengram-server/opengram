namespace MyTelegram.Domain.Sagas.Identities;

public class GroupCallMessageSagaLocator : DefaultSagaLocator<GroupCallMessageSaga, GroupCallMessageSagaId>
{
    protected override GroupCallMessageSagaId CreateSagaId(string requestId)
    {
        return new GroupCallMessageSagaId(requestId);
    }
}
