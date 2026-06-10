namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl;

///<summary>
/// Invoke a method within a business connection context.
/// See <a href="https://corefork.telegram.org/method/invokeWithBusinessConnection" />
///</summary>
internal sealed class InvokeWithBusinessConnectionHandler(
    IHandlerHelper handlerHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.RequestInvokeWithBusinessConnection, IObject>,
        IInvokeWithBusinessConnectionHandler
{
    protected override async Task<IObject> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.RequestInvokeWithBusinessConnection obj)
    {
        // Business connection ID is consumed by the transport/context layer.
        // This wrapper transparently delegates to the inner query.
        if (!handlerHelper.TryGetHandler(obj.Query.ConstructorId, out var handler))
        {
            throw new RpcException(new RpcError(400, "INPUT_METHOD_INVALID"));
        }

        return await handler.HandleAsync(input, obj.Query);
    }
}
