using IChannelParticipantResponseConverter =
    MyTelegram.Converters.Responses.Interfaces.IChannelParticipantResponseConverter;

namespace MyTelegram.Converters.Responses;

public class ChannelParticipantResponseService(
    ILayeredService<IChannelParticipantAdminResponseConverter> channelParticipantAdminLayeredService,
    ILayeredService<IChannelParticipantBannedResponseConverter> channelParticipantBannedLayeredService,
    ILayeredService<IChannelParticipantCreatorResponseConverter> channelParticipantCreatorLayeredService,
    ILayeredService<IChannelParticipantLeftResponseConverter> channelParticipantLeftLayeredService,
    ILayeredService<IChannelParticipantResponseConverter> channelParticipantLayeredService,
    ILayeredService<IChannelParticipantSelfResponseConverter> channelParticipantSelfLayeredService)
    : IChannelParticipantResponseService, ITransientDependency
{
    public IChannelParticipant ToLayeredData(IChannelParticipant latestLayerData, int layer)
    {
        switch (latestLayerData)
        {
            case TChannelParticipant channelParticipant:
                return channelParticipantLayeredService.GetConverter(layer).ToLayeredData(channelParticipant);
            case TChannelParticipantSelf channelParticipantSelf:
                return channelParticipantSelfLayeredService.GetConverter(layer).ToLayeredData(channelParticipantSelf);
            case TChannelParticipantAdmin channelParticipantAdmin:
                return channelParticipantAdminLayeredService.GetConverter(layer).ToLayeredData(channelParticipantAdmin);
            case TChannelParticipantBanned channelParticipantBanned:
                return channelParticipantBannedLayeredService.GetConverter(layer)
                    .ToLayeredData(channelParticipantBanned);
            case TChannelParticipantCreator channelParticipantCreator:
                return channelParticipantCreatorLayeredService.GetConverter(layer)
                    .ToLayeredData(channelParticipantCreator);
            case TChannelParticipantLeft channelParticipantLeft:
                return channelParticipantLeftLayeredService.GetConverter(layer).ToLayeredData(channelParticipantLeft);
        }

        return latestLayerData;
    }
}