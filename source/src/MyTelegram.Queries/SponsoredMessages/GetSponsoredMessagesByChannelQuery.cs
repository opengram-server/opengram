namespace MyTelegram.Queries.SponsoredMessages;

public class GetSponsoredMessagesByChannelQuery : IQuery<IReadOnlyCollection<ISponsoredMessageReadModel>>
{
    public long ChannelId { get; }
    public bool OnlyActive { get; }

    public GetSponsoredMessagesByChannelQuery(long channelId, bool onlyActive = true)
    {
        ChannelId = channelId;
        OnlyActive = onlyActive;
    }
}
