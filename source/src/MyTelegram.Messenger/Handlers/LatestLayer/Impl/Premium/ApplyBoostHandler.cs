// ReSharper disable All

using MyTelegram.Services.Services;
using MyTelegram.Schema.Premium;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Premium;

///<summary>
/// Apply one or more <a href="https://corefork.telegram.org/api/boost">boosts »</a> to a peer.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 BOOSTS_EMPTY No boost slots were specified.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// 400 SLOTS_EMPTY The specified slot list is empty.
/// See <a href="https://corefork.telegram.org/method/premium.applyBoost" />
///</summary>
internal sealed class ApplyBoostHandler(
    IPeerHelper peerHelper,
    IChannelAppService channelAppService,
    IUserAppService userAppService,
    ILogger<ApplyBoostHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Premium.RequestApplyBoost, MyTelegram.Schema.Premium.IMyBoosts>,
    Premium.IApplyBoostHandler
{
    protected override async Task<MyTelegram.Schema.Premium.IMyBoosts> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Premium.RequestApplyBoost obj)
    {
        // Resolve the target peer
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        if (peer.PeerId == 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }

        // Validate slots if provided
        if (obj.Slots is { Count: 0 })
        {
            throw new RpcException(RpcErrors.RpcErrors400.SlotsEmpty);
        }

        // Verify user has premium (boost requires premium)
        var userReadModel = await userAppService.GetAsync(input.UserId);
        if (userReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }

        // Verify target peer exists (channel/supergroup)
        if (peerHelper.IsChannelPeer(peer.PeerId))
        {
            var channelReadModel = await channelAppService.GetAsync(peer.PeerId);
            if (channelReadModel == null)
            {
                throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
            }
        }

        // Determine which slot IDs to use
        var slotIds = obj.Slots?.ToList() ?? new List<int> { 1 };
        if (slotIds.Count == 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.BoostsEmpty);
        }

        logger.LogInformation(
            "ApplyBoost: PeerId={PeerId}, Slots={Slots}, UserId={UserId}",
            peer.PeerId, string.Join(",", slotIds), input.UserId);

        var now = CurrentDate;
        var expiresDate = now + (30 * 24 * 60 * 60); // 30 days from now

        // Build response with boost slots assigned to the peer
        var myBoosts = new TVector<IMyBoost>();
        foreach (var slotId in slotIds)
        {
            myBoosts.Add(new TMyBoost
            {
                Slot = slotId,
                Peer = peerHelper.ToPeer(peer),
                Date = now,
                Expires = expiresDate,
                CooldownUntilDate = now + (7 * 24 * 60 * 60) // 7-day cooldown before reassignment
            });
        }

        // Add any remaining unassigned boost slots (Premium users get multiple)
        var totalSlots = userReadModel.Premium ? 4 : 1;
        for (var i = 1; i <= totalSlots; i++)
        {
            if (!slotIds.Contains(i))
            {
                myBoosts.Add(new TMyBoost
                {
                    Slot = i,
                    Date = 0,
                    Expires = 0
                });
            }
        }

        return new TMyBoosts
        {
            MyBoosts = myBoosts,
            Chats = [],
            Users = []
        };
    }
}
