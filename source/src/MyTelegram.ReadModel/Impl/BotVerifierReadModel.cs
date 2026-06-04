namespace MyTelegram.ReadModel.Impl;

public class BotVerifierReadModel : IBotVerifierReadModel,
    IAmReadModelFor<UserAggregate, UserId, BotVerifierCreatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, BotVerifierSettingsUpdatedEvent>
{
    public BotVerifierReadModel()
    {
    }

    public BotVerifierReadModel(
        long botUserId,
        long iconEmojiId,
        string companyName,
        bool canModifyCustomDescription,
        DateTime createdAt)
    {
        Id = $"bot_{botUserId}";
        BotUserId = botUserId;
        IconEmojiId = iconEmojiId;
        CompanyName = companyName;
        CanModifyCustomDescription = canModifyCustomDescription;
        IsActive = true;
        CreatedAt = createdAt;
    }

    public virtual string Id { get; set; } = default!;
    public virtual long? Version { get; set; }
    public long BotUserId { get; set; }
    public long IconEmojiId { get; set; }
    public string CompanyName { get; set; } = default!;
    public bool CanModifyCustomDescription { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, BotVerifierCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Id = $"bot_{evt.BotUserId}";
        BotUserId = evt.BotUserId;
        IconEmojiId = evt.IconEmojiId;
        CompanyName = evt.CompanyName;
        CanModifyCustomDescription = evt.CanModifyCustomDescription;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, BotVerifierSettingsUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        IconEmojiId = evt.IconEmojiId;
        CompanyName = evt.CompanyName;
        CanModifyCustomDescription = evt.CanModifyCustomDescription;
        UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}
