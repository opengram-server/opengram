namespace MyTelegram.Domain.Events.Channel;

public class
    ChannelCreatorCreatedEvent(
        RequestInfo requestInfo,
        long channelId,
        long userId,
        long inviterId,
        int date,
        bool isBroadcast
        )
    : RequestAggregateEvent2<ChannelMemberAggregate, ChannelMemberId>(requestInfo) //, IHasCorrelationId
{
    //

    public long ChannelId { get; } = channelId;
    public int Date { get; } = date;
    public bool IsBroadcast { get; } = isBroadcast;
    public long InviterId { get; } = inviterId;
    public long UserId { get; } = userId;

    //
}
