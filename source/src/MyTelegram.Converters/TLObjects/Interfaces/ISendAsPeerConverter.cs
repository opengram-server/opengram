using MyTelegram.Schema.Channels;

namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface ISendAsPeerConverter : ILayeredConverter
{
    ISendAsPeers ToSendAsPeers(IList<IChat> channels);
}