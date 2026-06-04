using MyTelegram.Schema.Channels;

namespace MyTelegram.Converters.TLObjects.LatestLayer;

public class SendAsPeerConverter : ISendAsPeerConverter, ITransientDependency
{
    
    public virtual int Layer => Layers.LayerLatest;

    public virtual ISendAsPeers ToSendAsPeers(IList<IChat> channels)
    {
        var sendAsPeers = new TSendAsPeers
        {
            Peers = new TVector<ISendAsPeer>(channels.Select(p => new TSendAsPeer
            {
                Peer = new TPeerChannel
                {
                    ChannelId = p.Id
                }
            })),
            Chats = [.. channels],
            Users = []
        };

        return sendAsPeers;
    }
}