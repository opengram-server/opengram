using MyTelegram.Domain.Aggregates.Chat;
using MyTelegram.Domain.Events.Chat;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.ReadModel.Impl;

public class ChatReadModel : IChatReadModel,
    IAmReadModelFor<ChatAggregate, ChatId, ChatCreatedEvent>,
    IAmReadModelFor<ChatAggregate, ChatId, ChatMemberAddedEvent>,
    IAmReadModelFor<ChatAggregate, ChatId, ChatMemberDeletedEvent>
{
    public string Id { get; set; } = null!;
    public long ChatId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public long CreatorId { get; private set; }
    public int Date { get; private set; }
    public int Version { get; private set; }
    public List<ChatMember> ChatMembers { get; private set; } = new();
    public bool Deactivated { get; private set; }
    public int? MigratedToChannelId { get; private set; }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChatAggregate, ChatId, ChatCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        ChatId = domainEvent.AggregateEvent.ChatId;
        Title = domainEvent.AggregateEvent.Title;
        CreatorId = domainEvent.AggregateEvent.CreatorId;
        Date = domainEvent.AggregateEvent.Date;
        Version = 1;
        
        // Add initial members
        ChatMembers = domainEvent.AggregateEvent.MemberUidList.Select(userId => new ChatMember
        {
            UserId = userId,
            InviterId = domainEvent.AggregateEvent.CreatorId,
            Date = domainEvent.AggregateEvent.Date
        }).ToList();
        
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChatAggregate, ChatId, ChatMemberAddedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var existingMember = ChatMembers.FirstOrDefault(m => m.UserId == domainEvent.AggregateEvent.UserId);
        if (existingMember == null)
        {
            ChatMembers.Add(new ChatMember
            {
                UserId = domainEvent.AggregateEvent.UserId,
                InviterId = domainEvent.AggregateEvent.InviterId,
                Date = domainEvent.AggregateEvent.Date
            });
        }
        Version++;
        
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChatAggregate, ChatId, ChatMemberDeletedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        ChatMembers.RemoveAll(m => m.UserId == domainEvent.AggregateEvent.UserId);
        Version++;
        
        return Task.CompletedTask;
    }
}

public class ChatMember
{
    public long UserId { get; set; }
    public long InviterId { get; set; }
    public int Date { get; set; }
}
