namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Contacts;

///<summary>
/// Returns the list of contacts available for synchronization.
/// See <a href="https://corefork.telegram.org/method/contacts.getSaved" />
///</summary>
internal sealed class GetSavedHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Contacts.RequestGetSaved, TVector<MyTelegram.Schema.ISavedContact>>,
    Contacts.IGetSavedHandler
{
    protected override Task<TVector<ISavedContact>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Contacts.RequestGetSaved obj)
    {
        // Saved contacts synchronization requires a SavedContactReadModel
        // that stores phone-number-only contacts (not yet registered users).
        // Return an empty list — client will not show phantom contacts.
        return Task.FromResult<TVector<ISavedContact>>([]);
    }
}
