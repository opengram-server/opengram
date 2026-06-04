using MyTelegram.Domain.Aggregates.Chat;

namespace MyTelegram.Domain.Events.Chat;

public class ChatAvailableReactionsChangedEvent : RequestAggregateEvent2<ChatAggregate, ChatId>
{
    public ChatAvailableReactionsChangedEvent(
        RequestInfo requestInfo,
        long chatId,
        ReactionType reactionType,
        bool allowCustomReaction,
        List<string>? availableReactions,
        int? reactionsLimit) : base(requestInfo)
    {
        ChatId = chatId;
        ReactionType = reactionType;
        AllowCustomReaction = allowCustomReaction;
        AvailableReactions = availableReactions;
        ReactionsLimit = reactionsLimit;
    }

    public long ChatId { get; }
    public ReactionType ReactionType { get; }
    public bool AllowCustomReaction { get; }
    public List<string>? AvailableReactions { get; }
    public int? ReactionsLimit { get; }
}
