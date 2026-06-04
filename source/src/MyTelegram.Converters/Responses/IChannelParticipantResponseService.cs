namespace MyTelegram.Converters.Responses;

public interface IChannelParticipantResponseService
{
    IChannelParticipant ToLayeredData(IChannelParticipant latestLayerData, int layer);
}