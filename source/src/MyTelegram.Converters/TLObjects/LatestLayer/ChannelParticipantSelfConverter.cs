namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class ChannelParticipantSelfConverter(IObjectMapper objectMapper)
    : IChannelParticipantSelfConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipant ToChannelParticipantSelf(IChannelMemberReadModel channelMemberReadModel)
    {
        return objectMapper.Map<IChannelMemberReadModel, TChannelParticipantSelf>(channelMemberReadModel);
    }
}