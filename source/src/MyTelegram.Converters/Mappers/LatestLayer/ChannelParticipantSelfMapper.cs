namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class ChannelParticipantSelfMapper
    : IObjectMapper<IChannelMemberReadModel, TChannelParticipantSelf>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public TChannelParticipantSelf Map(IChannelMemberReadModel source)
    {
        return Map(source, new TChannelParticipantSelf());
    }

    public TChannelParticipantSelf Map(
        IChannelMemberReadModel source,
        TChannelParticipantSelf destination
    )
    {
        destination.ViaRequest = source.ChatJoinType == ChatJoinType.ByRequest;
        destination.UserId = source.UserId;
        destination.InviterId = source.InviterId;
        destination.Date = source.Date;
        destination.SubscriptionUntilDate = source.SubscriptionUntilDate;

        return destination;
    }
}