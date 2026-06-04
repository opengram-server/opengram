namespace MyTelegram;

public class RpcException : Exception
{
    public RpcException(RpcError rpcError, long reqMsgId = 0) : base(rpcError.Message)
    {
        RpcError = rpcError;
        ReqMsgId = reqMsgId;
    }

    public RpcException(string message, RpcError rpcError, long reqMsgId = 0) : base(message)
    {
        RpcError = rpcError;
        ReqMsgId = reqMsgId;
    }

    public RpcException(string message, Exception inner, RpcError rpcError, long reqMsgId = 0) : base(message, inner)
    {
        RpcError = rpcError;
        ReqMsgId = reqMsgId;
    }

    public RpcError RpcError { get; }
    public long ReqMsgId { get; }
}