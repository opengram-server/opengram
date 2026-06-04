namespace MyTelegram.Queries.SponsoredMessages;

public class GetAllSponsoredMessagesQuery : IQuery<IReadOnlyCollection<ISponsoredMessageReadModel>>
{
    public bool? IsActive { get; }
    public long? ChannelId { get; }

    public GetAllSponsoredMessagesQuery(bool? isActive = null, long? channelId = null)
    {
        IsActive = isActive;
        ChannelId = channelId;
    }
}
