using MyTelegram.Converters.Responses.Interfaces.Payments;
using MyTelegram.Domain.Shared.Stars;
using MyTelegram.Messenger.Extensions;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Schema;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

public class GetStarsTransactionsHandler(
    IQueryProcessor queryProcessor,
    ILayeredService<IStarsStatusConverter> starsStatusLayeredService)
    : RpcResultObjectHandler<RequestGetStarsTransactions, IStarsStatus>,
    Payments.IGetStarsTransactionsHandler
{
    protected override async Task<IStarsStatus> HandleCoreAsync(IRequestInput input,
        RequestGetStarsTransactions obj)
    {
        var peerId = obj.Peer.GetPeerId();
        if (peerId == 0) peerId = input.UserId;

        var query = new MyTelegram.Queries.Stars.GetStarsTransactionsQuery
        {
            PeerId = peerId,
            IsInbound = obj.Inbound,
            IsOutbound = obj.Outbound,
            IsAscending = obj.Ascending,
            IsTon = obj.Ton,
            SubscriptionId = obj.SubscriptionId,
            Offset = obj.Offset,
            Limit = obj.Limit
        };

        var result = await queryProcessor.ProcessAsync(query);

        if (result == null)
        {
             return new TStarsStatus
            {
                Balance = new TStarsAmount { Amount = 0 },
                History = new TVector<IStarsTransaction>(),
                Subscriptions = new TVector<IStarsSubscription>(),
                Chats = new TVector<IChat>(),
                Users = new TVector<IUser>()
            };
        }

        return starsStatusLayeredService.GetConverter(input.Layer).ToStarsStatus(result);
    }
}
