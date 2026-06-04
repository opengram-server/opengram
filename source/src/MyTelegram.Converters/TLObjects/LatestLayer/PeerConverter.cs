using MyTelegram.Schema.Phone;

namespace MyTelegram.Converters.TLObjects.LatestLayer;

public class PeerConverter : IPeerConverter, ITransientDependency
{
    
    public virtual int Layer => Layers.LayerLatest;

    public IJoinAsPeers ToJoinAsPeers(
        IUser user,
        IChat? channel
    )
    {
        var peerList = new List<IPeer>();
        var peer = new TPeerUser { UserId = user.Id };
        peerList.Add(peer);
        if (channel != null)
        {
            peerList.Add(new TPeerChannel { ChannelId = channel.Id });
        }

        return new TJoinAsPeers
        {
            Chats = channel == null ? new TVector<IChat>() : new TVector<IChat>(channel),
            Peers = new TVector<IPeer>(peerList),
            Users = new TVector<IUser>(user)
        };
    }
}