namespace MyTelegram.ReadModel.Impl;

public class BotVerificationReadModel : IBotVerificationReadModel,
    IAmReadModelFor<UserAggregate, UserId, CustomVerificationSetEvent>,
    IAmReadModelFor<UserAggregate, UserId, CustomVerificationRemovedEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, ChannelCustomVerificationSetEvent>,
    IAmReadModelFor<ChannelAggregate, ChannelId, ChannelCustomVerificationRemovedEvent>
{
    public BotVerificationReadModel()
    {
    }

    public BotVerificationReadModel(
        long botVerifierId,
        VerificationTargetType targetType,
        long targetId,
        long iconEmojiId,
        string description,
        string? customDescription,
        DateTime verifiedAt)
    {
        Id = $"verification_{(int)targetType}_{targetId}";
        BotVerifierId = botVerifierId;
        TargetType = targetType;
        TargetId = targetId;
        IconEmojiId = iconEmojiId;
        Description = description;
        CustomDescription = customDescription;
        VerifiedAt = verifiedAt;
    }

    public virtual string Id { get; set; } = default!;
    public virtual long? Version { get; set; }
    public long BotVerifierId { get; set; }
    public VerificationTargetType TargetType { get; set; }
    public long TargetId { get; set; }
    public long IconEmojiId { get; set; }
    public string Description { get; set; } = default!;
    public string? CustomDescription { get; set; }
    public DateTime VerifiedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, CustomVerificationSetEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Id = $"verification_{(int)VerificationTargetType.User}_{evt.TargetUserId}";
        BotVerifierId = evt.BotVerifierId;
        TargetType = VerificationTargetType.User;
        TargetId = evt.TargetUserId;
        IconEmojiId = evt.IconEmojiId;
        Description = evt.Description;
        CustomDescription = evt.CustomDescription;
        VerifiedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, CustomVerificationRemovedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        context.MarkForDeletion();
        return Task.CompletedTask;
    }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<ChannelAggregate, ChannelId, ChannelCustomVerificationSetEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Id = $"verification_{(int)VerificationTargetType.Channel}_{evt.ChannelId}";
        BotVerifierId = evt.BotVerifierId;
        TargetType = VerificationTargetType.Channel;
        TargetId = evt.ChannelId;
        IconEmojiId = evt.IconEmojiId;
        Description = evt.Description;
        CustomDescription = evt.CustomDescription;
        VerifiedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<ChannelAggregate, ChannelId, ChannelCustomVerificationRemovedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        context.MarkForDeletion();
        return Task.CompletedTask;
    }
}
