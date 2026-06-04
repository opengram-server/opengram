namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Clear all peer-specific autosave settings.
/// See <a href="https://corefork.telegram.org/method/account.deleteAutoSaveExceptions" />
///</summary>
internal sealed class DeleteAutoSaveExceptionsHandler : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestDeleteAutoSaveExceptions, IBool>,
    Account.IDeleteAutoSaveExceptionsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestDeleteAutoSaveExceptions obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
