namespace MyTelegram.Domain.Aggregates.RpcResult;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<RpcResultId>))]
public class RpcResultId(string value) : Identity<RpcResultId>(value)
{
    public static RpcResultId Create(long userId,long reqMsgId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"rpcresult-{userId}-{reqMsgId}");
    }
}
