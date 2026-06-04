using MyTelegram.Schema;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Services;

namespace MyTelegram.Messenger.Services.Impl;
public class RpcErrorHelper(IObjectMessageSender objectMessageSender) : IRpcErrorHelper, ITransientDependency
{
    public Task ThrowRpcErrorAsync(MyTelegram.Messenger.Services.Impl.RequestInfo requestInfo, RpcError rpcError)
    {
        // Convert from local RequestInfo to global RequestInfo
        var globalRequestInfo = new MyTelegram.RequestInfo(
            0, // reqMsgId - default value
            requestInfo.UserId,
            0, // accessHashKeyId - default value
            0, // authKeyId - default value
            0, // permAuthKeyId - default value
            Guid.Empty, // requestId - must be Guid, not string
            0, // layer - default value
            0, // date - default value
            MyTelegram.DeviceType.Unknown // deviceType - default value
        );
        return objectMessageSender.SendRpcMessageToClientAsync(globalRequestInfo, rpcError.ToRpcError());
    }

    public Task ThrowRpcErrorAsync(IRequestInput requestInfo, RpcError rpcError)
    {
        return objectMessageSender.SendRpcMessageToClientAsync(requestInfo.ToRequestInfo(), rpcError.ToRpcError());
    }
}
