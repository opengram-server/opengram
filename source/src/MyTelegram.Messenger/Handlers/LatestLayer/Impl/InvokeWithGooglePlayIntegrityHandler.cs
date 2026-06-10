namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl;

///<summary>
/// Invoke with Google Play Integrity token.
/// See <a href="https://corefork.telegram.org/method/invokeWithGooglePlayIntegrity" />
///</summary>
internal sealed class InvokeWithGooglePlayIntegrityHandler(
    IHandlerHelper handlerHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.RequestInvokeWithGooglePlayIntegrity, IObject>,
        IInvokeWithGooglePlayIntegrityHandler
{
    protected override async Task<IObject> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.RequestInvokeWithGooglePlayIntegrity obj)
    {
        if (!handlerHelper.TryGetHandler(obj.Query.ConstructorId, out var handler))
        {
            throw new RpcException(new RpcError(400, "INPUT_METHOD_INVALID"));
        }

        return await handler.HandleAsync(input, obj.Query);
    }
}
