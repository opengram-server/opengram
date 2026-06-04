namespace MyTelegram.Converters.Responses.Interfaces;

public interface IChannelParticipantBannedResponseConverter
    : IResponseConverter<
        TChannelParticipantBanned,
        IChannelParticipant
    >
{
}