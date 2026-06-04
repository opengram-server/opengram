namespace MyTelegram;

public static class RpcErrorExtensions
{
    public static void ThrowRpcError(this RpcError rpcError, long reqMsgId = 0)
    {
        throw new RpcException(rpcError, reqMsgId);
    }

    public static void ThrowRpcError(this RpcError rpcError, int xToReplace, long reqMsgId = 0)
    {
        throw new RpcException(rpcError with { Message = string.Format(rpcError.Message, xToReplace) });
    }
}