namespace MyTelegram.Messenger.Services.Interfaces;

public interface IRpcErrorHelper
{
    Task ThrowRpcErrorAsync(MyTelegram.Messenger.Services.Impl.RequestInfo requestInfo, RpcError rpcError);
    Task ThrowRpcErrorAsync(IRequestInput requestInfo, RpcError rpcError);
}