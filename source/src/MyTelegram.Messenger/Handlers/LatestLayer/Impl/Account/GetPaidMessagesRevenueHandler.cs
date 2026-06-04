// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.Account;

///<summary>
/// See <a href="https://corefork.telegram.org/method/account.getPaidMessagesRevenue" />
///</summary>
internal sealed class GetPaidMessagesRevenueHandler : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetPaidMessagesRevenue, MyTelegram.Schema.Account.IPaidMessagesRevenue>,
    Account.IGetPaidMessagesRevenueHandler
{
    protected override Task<MyTelegram.Schema.Account.IPaidMessagesRevenue> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetPaidMessagesRevenue obj)
    {
        return Task.FromResult<IPaidMessagesRevenue>(new TPaidMessagesRevenue
        {
            StarsAmount = 10000
        });
    }
}
