namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl;

///<summary>
/// Invoke with messages range context.
/// See <a href="https://corefork.telegram.org/method/invokeWithMessagesRange" />
///</summary>
internal sealed class InvokeWithMessagesRangeHandler(
    IHandlerHelper handlerHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.RequestInvokeWithMessagesRange, IObject>,
        IInvokeWithMessagesRangeHandler
{
    protected override async Task<IObject> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.RequestInvokeWithMessagesRange obj)
    {
        if (!handlerHelper.TryGetHandler(obj.Query.ConstructorId, out var handler))
        {
            throw new RpcException(new RpcError(400, "INPUT_METHOD_INVALID"));
        }

        return await handler.HandleAsync(input, obj.Query);
    }
}
