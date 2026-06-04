namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IChannelParticipantSelfConverter : ILayeredConverter
{
    IChannelParticipant ToChannelParticipantSelf(IChannelMemberReadModel channelMemberReadModel);
}