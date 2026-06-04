using IChannelParticipant = MyTelegram.Schema.Channels.IChannelParticipant;
using TChannelParticipant = MyTelegram.Schema.Channels.TChannelParticipant;

namespace MyTelegram.Converters.Responses.Interfaces.Channels;

public interface IChannelParticipantResponseConverter
    : IResponseConverter<
        TChannelParticipant,
        IChannelParticipant
    >
{
}