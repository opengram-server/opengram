using MyTelegram.Schema.Channels;

namespace MyTelegram.Converters.Responses.Interfaces.Channels;

public interface IChannelParticipantsResponseConverter
    : IResponseConverter<
        TChannelParticipants,
        IChannelParticipants
    >
{
}