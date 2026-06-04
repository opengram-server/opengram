// ReSharper disable All

using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Get a list of peers that can be used to join a group call, presenting yourself as a specific user/channel.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/phone.getGroupCallJoinAs" />
///</summary>
internal sealed class GetGroupCallJoinAsHandler(
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestGetGroupCallJoinAs, MyTelegram.Schema.Phone.IJoinAsPeers>,
    Phone.IGetGroupCallJoinAsHandler
{
    protected override async Task<MyTelegram.Schema.Phone.IJoinAsPeers> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestGetGroupCallJoinAs obj)
    {
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        var peers = new List<IPeer> { new TPeerUser { UserId = input.UserId } };
        
        if (peer.PeerType == PeerType.Channel)
        {
            try
            {
                var channelMember = await queryProcessor.ProcessAsync(
                    new GetChannelMemberByUserIdQuery(peer.PeerId, input.UserId), default);
                
                if (channelMember?.IsAdmin == true)
                {
                    peers.Add(new TPeerChannel { ChannelId = peer.PeerId });
                }
            }
            catch { }
        }
        
        return new MyTelegram.Schema.Phone.TJoinAsPeers
        {
            Peers = new TVector<IPeer>(peers),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };
    }
}
