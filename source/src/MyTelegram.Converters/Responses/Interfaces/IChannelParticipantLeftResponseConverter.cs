namespace MyTelegram.Converters.Responses.Interfaces;

public interface IChannelParticipantLeftResponseConverter
    : IResponseConverter<
        TChannelParticipantLeft,
        IChannelParticipant
    >
{
}