namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// See <a href="https://corefork.telegram.org/method/phone.createConferenceCall" />
///</summary>
internal sealed class CreateConferenceCallHandler : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestCreateConferenceCall, MyTelegram.Schema.IUpdates>,
    Phone.ICreateConferenceCallHandler
{
    protected override Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestCreateConferenceCall obj)
    {
        throw new NotImplementedException();
    }
}
