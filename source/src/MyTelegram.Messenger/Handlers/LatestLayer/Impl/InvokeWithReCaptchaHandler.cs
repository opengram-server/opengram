namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl;

///<summary>
/// Invoke with ReCaptcha token for verification.
/// See <a href="https://corefork.telegram.org/method/invokeWithReCaptcha" />
///</summary>
internal sealed class InvokeWithReCaptchaHandler(
    IHandlerHelper handlerHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.RequestInvokeWithReCaptcha, IObject>,
        IInvokeWithReCaptchaHandler
{
    protected override async Task<IObject> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.RequestInvokeWithReCaptcha obj)
    {
        if (!handlerHelper.TryGetHandler(obj.Query.ConstructorId, out var handler))
        {
            throw new RpcException(new RpcError(400, "INPUT_METHOD_INVALID"));
        }

        return await handler.HandleAsync(input, obj.Query);
    }
}
