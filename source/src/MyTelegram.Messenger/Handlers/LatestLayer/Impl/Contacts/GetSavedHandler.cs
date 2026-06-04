namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Contacts;

///<summary>
/// Get all contacts, requires a <a href="https://corefork.telegram.org/api/takeout">takeout session, see here » for more info</a>.
/// See <a href="https://corefork.telegram.org/method/contacts.getSaved" />
///</summary>
internal sealed class GetSavedHandler : RpcResultObjectHandler<MyTelegram.Schema.Contacts.RequestGetSaved, TVector<MyTelegram.Schema.ISavedContact>>,
    Contacts.IGetSavedHandler
{
    protected override Task<TVector<MyTelegram.Schema.ISavedContact>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Contacts.RequestGetSaved obj)
    {
        throw new NotImplementedException();
    }
}
