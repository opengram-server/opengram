namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Users;

///<summary>
/// Notify the user that the sent passport data contains some errors.
/// See <a href="https://corefork.telegram.org/method/users.setSecureValueErrors" />
///</summary>
internal sealed class SetSecureValueErrorsHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Users.RequestSetSecureValueErrors, IBool>,
        Users.ISetSecureValueErrorsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Users.RequestSetSecureValueErrors obj)
    {
        // Passport/secure value errors - requires Telegram Passport infrastructure
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
