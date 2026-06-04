namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class ChannelParticipantMapper
    : IObjectMapper<IChannelMemberReadModel, TChannelParticipant>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public TChannelParticipant Map(IChannelMemberReadModel source)
    {
        return Map(source, new TChannelParticipant());
    }

    public TChannelParticipant Map(
        IChannelMemberReadModel source,
        TChannelParticipant destination
    )
    {
        destination.UserId = source.UserId;
        destination.Date = source.Date;
        destination.SubscriptionUntilDate = source.SubscriptionUntilDate;

        return destination;
    }
}