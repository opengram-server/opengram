using MyTelegram.Domain.Events.Privacy;

namespace MyTelegram.ReadModel.Impl;

public partial class PrivacyReadModel :
    IAmReadModelFor<UserAggregate, UserId, PrivacyRulesUpdatedEvent>
{
    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, PrivacyRulesUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        
        Id = PrivacyId.Create(evt.UserId, evt.PrivacyType);
        UserId = evt.UserId;
        PrivacyType = evt.PrivacyType;
        PrivacyValueDataList = evt.Rules.ToList();
        
        return Task.CompletedTask;
    }
}
