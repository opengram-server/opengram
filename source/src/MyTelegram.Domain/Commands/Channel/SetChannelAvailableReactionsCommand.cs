namespace MyTelegram.Domain.Commands.Channel;

public class SetChannelAvailableReactionsCommand : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>
{
    public SetChannelAvailableReactionsCommand(
        ChannelId aggregateId,
        RequestInfo requestInfo,
        ReactionType reactionType,
        bool allowCustomReaction,
        List<string>? availableReactions,
        int? reactionsLimit) : base(aggregateId, requestInfo)
    {
        ReactionType = reactionType;
        AllowCustomReaction = allowCustomReaction;
        AvailableReactions = availableReactions;
        ReactionsLimit = reactionsLimit;
    }

    public ReactionType ReactionType { get; }
    public bool AllowCustomReaction { get; }
    public List<string>? AvailableReactions { get; }
    public int? ReactionsLimit { get; }
}
