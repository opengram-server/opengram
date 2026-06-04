// ReSharper disable All

using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Set the default peer that will be used to join a group call in a specific dialog.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 JOIN_AS_PEER_INVALID The specified peer cannot be used to join a group call.
/// See <a href="https://corefork.telegram.org/method/phone.saveDefaultGroupCallJoinAs" />
///</summary>
internal sealed class SaveDefaultGroupCallJoinAsHandler(IPeerHelper peerHelper) 
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestSaveDefaultGroupCallJoinAs, IBool>,
    Phone.ISaveDefaultGroupCallJoinAsHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestSaveDefaultGroupCallJoinAs obj)
    {
        // Get the peer (channel/chat) where we want to set default join-as
        var channelPeer = peerHelper.GetPeer(obj.Peer, input.UserId);
        
        // Get the join-as peer (who we want to join as)
        var joinAsPeer = peerHelper.GetPeer(obj.JoinAs, input.UserId);
        
        // Validate join-as peer
        // User can join as themselves, or as a channel they own
        if (joinAsPeer.PeerType == PeerType.User)
        {
            // Can only join as self
            if (joinAsPeer.PeerId != input.UserId)
            {
                RpcErrors.RpcErrors400.JoinAsPeerInvalid.ThrowRpcError();
                return null!;
            }
        }
        else if (joinAsPeer.PeerType == PeerType.Channel)
        {
            // TODO: Check if user is owner/admin of this channel
            // For now, we accept it
        }
        else
        {
            // Can't join as a regular chat
            RpcErrors.RpcErrors400.JoinAsPeerInvalid.ThrowRpcError();
            return null!;
        }
        
        // TODO: Store this preference in database
        // For now, just return success since the actual join-as peer
        // is validated during JoinGroupCall anyway
        
        return await Task.FromResult<IBool>(new TBoolTrue());
    }
}
