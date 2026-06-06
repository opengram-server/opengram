namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Contacts;

///<summary>
/// Generates a temporary profile link for sharing.
/// See <a href="https://corefork.telegram.org/method/contacts.exportContactToken" />
///</summary>
internal sealed class ExportContactTokenHandler(
    IRandomHelper randomHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Contacts.RequestExportContactToken, MyTelegram.Schema.IExportedContactToken>,
    Contacts.IExportContactTokenHandler
{
    protected override Task<IExportedContactToken> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Contacts.RequestExportContactToken obj)
    {
        // Generate a random URL token for sharing
        var token = $"{input.UserId}_{randomHelper.NextLong():x}";

        return Task.FromResult<IExportedContactToken>(new TExportedContactToken
        {
            Url = $"https://t.me/contact/{token}",
            Expires = CurrentDate + 86400  // Valid for 24 hours
        });
    }
}
