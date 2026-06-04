namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class ChannelParticipantConverter(IObjectMapper objectMapper)
    : IChannelParticipantConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipant ToChatParticipant(IChannelMemberReadModel channelMemberReadModel)
    {
        return objectMapper.Map<IChannelMemberReadModel, TChannelParticipant>(channelMemberReadModel);
    }
}