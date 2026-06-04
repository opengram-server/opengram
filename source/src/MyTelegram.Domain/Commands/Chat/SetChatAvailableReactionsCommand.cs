using MyTelegram.Domain.Aggregates.Chat;

namespace MyTelegram.Domain.Commands.Chat;

public class SetChatAvailableReactionsCommand : RequestCommand2<ChatAggregate, ChatId, IExecutionResult>
{
    public SetChatAvailableReactionsCommand(
        ChatId aggregateId,
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
