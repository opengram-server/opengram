namespace MyTelegram.Domain.Events.Privacy;

public class PrivacyRulesUpdatedEvent(
    RequestInfo requestInfo,
    long userId,
    PrivacyType privacyType,
    IReadOnlyList<PrivacyValueData> rules)
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public long UserId { get; } = userId;
    public PrivacyType PrivacyType { get; } = privacyType;
    public IReadOnlyList<PrivacyValueData> Rules { get; } = rules;
}
