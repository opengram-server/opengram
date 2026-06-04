namespace MyTelegram.Domain.Events.Channel;

public class ChannelAdminEditedEvent2(RequestInfo requestInfo, long channelId, long userId, int adminRights, string rank,bool isAdmin) : RequestAggregateEvent2<ChannelMemberAggregate, ChannelMemberId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long UserId { get; } = userId;
    public int AdminRights { get; } = adminRights;
    public string Rank { get; } = rank;

    public bool IsAdmin { get; } = isAdmin;
    //public int EditDate { get; } = editDate;
    //public long? PromotedBy { get; } = promotedBy;
}