using MyTelegram.Domain;

namespace MyTelegram.ReadModel.Impl;

public class UserFrozenAccountReadModel : IUserFrozenAccountReadModel,
    IAmReadModelFor<UserAggregate, UserId, UserAccountFrozenEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserAccountUnfrozenEvent>
{
    public string Id { get; set; } = null!;
    public long UserId { get; private set; }
    
    public int FreezeSinceDate { get; private set; }
    public int FreezeUntilDate { get; private set; }
    
    public FreezeReason Reason { get; private set; }
    public FreezeStatus Status { get; private set; }
    
    public string? AppealUrl { get; private set; }
    public DateTime? AppealSubmittedDate { get; private set; }
    public string? AppealText { get; private set; }
    
    public long? ModeratorUserId { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }
    
    public string? FreezeNote { get; private set; }
    public Dictionary<string, string>? Metadata { get; private set; }
    
    public long? Version { get; set; }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserAccountFrozenEvent> domainEvent, CancellationToken cancellationToken)
    {
        Id = $"frozen-{domainEvent.AggregateEvent.UserId}";
        UserId = domainEvent.AggregateEvent.UserId;
        FreezeSinceDate = domainEvent.AggregateEvent.FreezeSinceDate;
        FreezeUntilDate = domainEvent.AggregateEvent.FreezeUntilDate;
        Reason = domainEvent.AggregateEvent.Reason;
        Status = FreezeStatus.Active;
        AppealUrl = domainEvent.AggregateEvent.AppealUrl;
        ModeratorUserId = domainEvent.AggregateEvent.ModeratorUserId;
        FreezeNote = domainEvent.AggregateEvent.Note;
        CreatedDate = DateTime.UtcNow;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserAccountUnfrozenEvent> domainEvent, CancellationToken cancellationToken)
    {
        Status = FreezeStatus.Approved;
        LastModifiedDate = DateTime.UtcNow;

        return Task.CompletedTask;
    }
}
