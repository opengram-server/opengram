namespace MyTelegram.Domain.Events.RpcResult;

public class RpcResultCreatedEvent(
    RequestInfo requestInfo,
    byte[] rpcData,
    int date) : RequestAggregateEvent2<RpcResultAggregate, RpcResultId>(requestInfo)
{
    public byte[] RpcData { get; } = rpcData;
    public int Date { get; } = date;
}