namespace MyTelegram.Domain.Events.Channel;

public class ChannelAvailableReactionsChangedEvent : RequestAggregateEvent2<ChannelAggregate, ChannelId>
{
    public ChannelAvailableReactionsChangedEvent(
        RequestInfo requestInfo,
        long channelId,
        ReactionType reactionType,
        bool allowCustomReaction,
        List<string>? availableReactions,
        int? reactionsLimit) : base(requestInfo)
    {
        ChannelId = channelId;
        ReactionType = reactionType;
        AllowCustomReaction = allowCustomReaction;
        AvailableReactions = availableReactions;
        ReactionsLimit = reactionsLimit;
    }

    public long ChannelId { get; }
    public ReactionType ReactionType { get; }
    public bool AllowCustomReaction { get; }
    public List<string>? AvailableReactions { get; }
    public int? ReactionsLimit { get; }
}
