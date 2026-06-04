using Microsoft.Extensions.Logging;
using MyTelegram.Converters.Responses.Interfaces.Payments;
using MyTelegram.Domain.Shared.Stars;
using MyTelegram.Schema;
using MyTelegram.Schema.Payments;
using MyTelegram.Queries.Stars;
using MyTelegram.Messenger.Extensions;
using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// Get the current <a href="https://corefork.telegram.org/api/stars">Telegram Stars balance</a> of the current account (with peer=<a href="https://corefork.telegram.org/constructor/inputPeerSelf">inputPeerSelf</a>), or the stars balance of the bot specified in <code>peer</code>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/payments.getStarsStatus" />
///</summary>
internal sealed class GetStarsStatusHandler(
    ILogger<GetStarsStatusHandler> logger,
    IQueryProcessor queryProcessor,
    ILayeredService<IStarsStatusConverter> starsStatusLayeredService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetStarsStatus, MyTelegram.Schema.Payments.IStarsStatus>,
    Payments.IGetStarsStatusHandler
{
    protected override async Task<MyTelegram.Schema.Payments.IStarsStatus> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetStarsStatus obj)
    {
        logger.LogInformation("Getting stars status for user {UserId}, peer {PeerId}, isTon: {IsTon}", 
            input.UserId, obj.Peer, obj.Ton);

        // Validate peer
        var peerId = obj.Peer.GetPeerId();
        if (peerId == 0 && obj.Peer is TInputPeerSelf)
        {
            peerId = input.UserId;
        }
        
        if (peerId == 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }

        // Check if user has permission to view this peer's stars status
        if (peerId != input.UserId)
        {
            var userFull = await queryProcessor.ProcessAsync(new Services.Impl.GetUserFullQuery(peerId));
            if (userFull == null || !CanViewStarsStatus(input.UserId, peerId, userFull))
            {
                throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
            }
        }

        // Get stars status from database
        var starsStatus = await queryProcessor.ProcessAsync(new MyTelegram.Queries.Stars.GetStarsStatusQuery(peerId, obj.Ton));

        if (starsStatus == null)
        {
            // Return empty status if not found
            return CreateEmptyStarsStatus();
        }

        // Convert domain model to schema
        var result = starsStatusLayeredService.GetConverter(input.Layer).ToStarsStatus(starsStatus);

        logger.LogInformation("Retrieved stars status: balance {Balance}, transactions {TransactionCount} for peer {PeerId}", 
            starsStatus.Balance, starsStatus.History?.Count ?? 0, peerId);

        return result;
    }

    private bool CanViewStarsStatus(long requestUserId, long peerId, IUserFullReadModel userFull)
    {
        // Users can view their own stars status
        if (requestUserId == peerId)
        {
            return true;
        }

        // For other users, only allow if they have public revenue sharing enabled
        // This would need to be checked via user settings/permissions
        // For now, return false for privacy
        return false;
    }

    private bool IsBotAdmin(long userId, long botId)
    {
        // Implementation needed to check if user is bot admin
        // This would involve checking bot permissions
        return false; // Placeholder
    }

    private IStarsStatus CreateEmptyStarsStatus()
    {
        return new TStarsStatus
        {
            Balance = new TStarsAmount { Amount = 0 },
            Subscriptions = new TVector<IStarsSubscription>(),
            History = new TVector<IStarsTransaction>(),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };
    }
}
