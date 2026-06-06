namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl;

///<summary>
/// Invoke method with APNS secret for iOS push notifications.
/// See <a href="https://corefork.telegram.org/method/invokeWithApnsSecret" />
///</summary>
internal sealed class InvokeWithApnsSecretHandler(
    IHandlerHelper handlerHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.RequestInvokeWithApnsSecret, IObject>,
        IInvokeWithApnsSecretHandler
{
    protected override async Task<IObject> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.RequestInvokeWithApnsSecret obj)
    {
        // APNS secret is used by the transport layer for push notifications.
        // This wrapper transparently delegates to the inner query.
        if (!handlerHelper.TryGetHandler(obj.Query.ConstructorId, out var handler))
        {
            throw new RpcException(new RpcError(400, "INPUT_METHOD_INVALID"));
        }

        return await handler.HandleAsync(input, obj.Query);
    }
}
