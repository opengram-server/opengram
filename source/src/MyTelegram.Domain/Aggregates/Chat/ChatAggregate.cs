using MyTelegram.Domain.Events.Chat;

namespace MyTelegram.Domain.Aggregates.Chat;

public class ChatAggregate : AggregateRoot<ChatAggregate, ChatId>
{
    private readonly ChatState _state = new();

    public ChatAggregate(ChatId id) : base(id)
    {
        Register(_state);
    }

    public void CreateChat(
        long chatId,
        string title,
        long creatorId,
        List<long> memberUidList,
        int date)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);

        Emit(new ChatCreatedEvent(
            chatId,
            title,
            creatorId,
            memberUidList,
            date));
    }

    public void AddMember(long userId, long inviterId, int date)
    {
        if (_state.MemberUidList.Contains(userId))
        {
            return;
        }

        Emit(new ChatMemberAddedEvent(userId, inviterId, date));
    }

    public void DeleteMember(long userId)
    {
        if (!_state.MemberUidList.Contains(userId))
        {
            return;
        }

        Emit(new ChatMemberDeletedEvent(userId));
    }

    public void SetAvailableReactions(
        RequestInfo requestInfo,
        ReactionType reactionType,
        bool allowCustomReaction,
        List<string>? availableReactions,
        int? reactionsLimit)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        Emit(new ChatAvailableReactionsChangedEvent(
            requestInfo,
            _state.ChatId,
            reactionType,
            allowCustomReaction,
            availableReactions,
            reactionsLimit));
    }
}
