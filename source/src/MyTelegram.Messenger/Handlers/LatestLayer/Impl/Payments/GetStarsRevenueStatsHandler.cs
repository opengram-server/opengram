using MyTelegram.Schema.Payments;
using MyTelegram.Queries.Stars;
using MyTelegram.Messenger.Extensions;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// Get <a href="https://corefork.telegram.org/api/stars">Telegram Star revenue statistics</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/payments.getStarsRevenueStats" />
///</summary>
internal sealed class GetStarsRevenueStatsHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetStarsRevenueStats, MyTelegram.Schema.Payments.IStarsRevenueStats>,
    Payments.IGetStarsRevenueStatsHandler
{
    private readonly IQueryProcessor _queryProcessor;

    public GetStarsRevenueStatsHandler(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    protected override async Task<MyTelegram.Schema.Payments.IStarsRevenueStats> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetStarsRevenueStats obj)
    {
        var peerId = obj.Peer.GetPeerId();
        if (peerId == 0 && obj.Peer is TInputPeerSelf)
        {
            peerId = input.UserId;
        }

        var balance = 0L;
        
        var starsStatus = await _queryProcessor.ProcessAsync(new GetStarsStatusQuery(peerId), CancellationToken.None);
        if (starsStatus != null)
        {
            balance = starsStatus.Balance;
        }

        return new TStarsRevenueStats
        {
            Status = new TStarsRevenueStatus
            {
                AvailableBalance = new TStarsAmount
                {
                    Amount = balance
                },
                CurrentBalance = new TStarsAmount
                {
                    Amount = balance
                },
                OverallRevenue = new TStarsAmount
                {
                    Amount = balance
                },
                WithdrawalEnabled = false
            },
            RevenueGraph = new TStatsGraphError
            {
                Error = "Not implemented"
            },
            UsdRate = 1
        };
    }
}
