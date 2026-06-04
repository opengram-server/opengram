namespace MyTelegram.Domain.Sagas.Events;

public class InviteToChannelCompletedSagaEvent(
    RequestInfo requestInfo,
    long channelId,
    long inviterId,
    bool broadcast,
    IReadOnlyCollection<long> memberUserIds,
    IReadOnlyCollection<long> botUserIds,
    bool hasLink,
    ChatJoinType chatJoinType
    )
    : RequestAggregateEvent2<InviteToChannelSaga, InviteToChannelSagaId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long InviterId { get; } = inviterId;
    public bool Broadcast { get; } = broadcast;
    public IReadOnlyCollection<long> MemberUserIds { get; } = memberUserIds;
    public IReadOnlyCollection<long> BotUserIds { get; } = botUserIds;
    public bool HasLink { get; } = hasLink;
    public ChatJoinType ChatJoinType { get; } = chatJoinType;
}
