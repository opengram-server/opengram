// ReSharper disable All

namespace MyTelegram.Handlers;

///<summary>
/// Official clients only, invoke with Google Play Integrity token.
/// See <a href="https://corefork.telegram.org/method/invokeWithGooglePlayIntegrity" />
///</summary>
internal sealed class InvokeWithGooglePlayIntegrityHandler : RpcResultObjectHandler<MyTelegram.Schema.RequestInvokeWithGooglePlayIntegrity, IObject>,
    IInvokeWithGooglePlayIntegrityHandler
{
    protected override Task<IObject> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.RequestInvokeWithGooglePlayIntegrity obj)
    {
        throw new NotImplementedException();
    }
}
