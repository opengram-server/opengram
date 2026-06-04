using MyTelegram.Schema.Phone;

namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IPeerConverter : ILayeredConverter
{
    IJoinAsPeers ToJoinAsPeers(
        //IUserReadModel userReadModel,
        IUser user,
        IChat? channel
        //IChannelReadModel? channelReadModel,
        //IChatReadModel? chatReadModel
    );
}