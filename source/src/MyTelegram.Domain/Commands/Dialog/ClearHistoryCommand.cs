namespace MyTelegram.Domain.Commands.Dialog;

public class ClearHistoryCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    bool revoke,
    string messageActionData,
    long randomId,
    List<int> messageIdListToBeDelete,
    int nextMaxId)
    : RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo) //, IHasCorrelationId
{
    //public bool Revoke { get; }

    //Revoke = revoke; 

    public string MessageActionData { get; } = messageActionData;
    public List<int> MessageIdListToBeDelete { get; } = messageIdListToBeDelete;
    public int NextMaxId { get; } = nextMaxId;
    public long RandomId { get; } = randomId;
    public bool Revoke { get; } = revoke;
}
