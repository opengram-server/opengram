// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.Account;

///<summary>
/// See <a href="https://corefork.telegram.org/method/account.addNoPaidMessagesException" />
///</summary>
internal sealed class AddNoPaidMessagesExceptionHandler : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestAddNoPaidMessagesException, IBool>,
    Account.IAddNoPaidMessagesExceptionHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestAddNoPaidMessagesException obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
