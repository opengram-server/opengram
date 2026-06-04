using MyTelegram.Domain.Events.Chat;

namespace MyTelegram.Domain.Aggregates.Chat;

public class ChatState : AggregateState<ChatAggregate, ChatId, ChatState>,
    IApply<ChatCreatedEvent>,
    IApply<ChatMemberAddedEvent>,
    IApply<ChatMemberDeletedEvent>
{
    public long ChatId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public long CreatorId { get; private set; }
    public List<long> MemberUidList { get; private set; } = new();
    public int Date { get; private set; }

    public void Apply(ChatCreatedEvent e)
    {
        ChatId = e.ChatId;
        Title = e.Title;
        CreatorId = e.CreatorId;
        MemberUidList = e.MemberUidList.ToList();
        Date = e.Date;
    }

    public void Apply(ChatMemberAddedEvent e)
    {
        if (!MemberUidList.Contains(e.UserId))
        {
            MemberUidList.Add(e.UserId);
        }
    }

    public void Apply(ChatMemberDeletedEvent e)
    {
        MemberUidList.Remove(e.UserId);
    }
}
