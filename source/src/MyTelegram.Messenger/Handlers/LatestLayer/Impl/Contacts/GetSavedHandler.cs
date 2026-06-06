namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Contacts;

///<summary>
/// Returns the list of contacts with saved (non-deleted) Telegram accounts.
/// See <a href="https://corefork.telegram.org/method/contacts.getSaved" />
///</summary>
internal sealed class GetSavedHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Contacts.RequestGetSaved, TVector<MyTelegram.Schema.ISavedContact>>,
    Contacts.IGetSavedHandler
{
    protected override Task<TVector<ISavedContact>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Contacts.RequestGetSaved obj)
    {
        // Return an empty saved contacts list — the full contact sync
        // happens via contacts.getContacts. This endpoint is for the
        // subset that have been explicitly saved.
        return Task.FromResult<TVector<ISavedContact>>([]);
    }
}
