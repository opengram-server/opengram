using MyTelegram.Domain;

namespace MyTelegram.ReadModel.Impl;

public class UserFrozenAppealHistoryReadModel : IUserFrozenAppealHistoryReadModel,
    IAmReadModelFor<UserAggregate, UserId, UserFrozenAppealSubmittedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserFrozenAppealReviewedEvent>
{
    public string Id { get; set; } = null!;
    public long UserId { get; private set; }
    public string AppealId { get; private set; } = null!;
    
    public DateTime SubmittedDate { get; private set; }
    public string AppealText { get; private set; } = null!;
    public string? CaptchaToken { get; private set; }
    
    public AppealStatus Status { get; private set; }
    public DateTime? ReviewedDate { get; private set; }
    public long? ReviewerModeratorId { get; private set; }
    public string? ReviewNote { get; private set; }
    
    public string? UserName { get; private set; }
    public Dictionary<string, string>? Answers { get; private set; }
    
    public long? Version { get; set; }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserFrozenAppealSubmittedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Id = $"appeal-{evt.AppealId}";
        UserId = evt.UserId;
        AppealId = evt.AppealId;
        AppealText = evt.AppealText;
        CaptchaToken = evt.CaptchaToken;
        UserName = evt.UserName;
        Answers = evt.Answers;
        Status = AppealStatus.Pending;
        SubmittedDate = DateTime.UtcNow;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserFrozenAppealReviewedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Status = evt.Status;
        ReviewerModeratorId = evt.ModeratorUserId;
        ReviewNote = evt.ReviewNote;
        ReviewedDate = DateTime.UtcNow;

        return Task.CompletedTask;
    }
}
