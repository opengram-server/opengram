namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IChannelParticipantConverter : ILayeredConverter
{
    IChannelParticipant ToChatParticipant(IChannelMemberReadModel channelMemberReadModel);
}