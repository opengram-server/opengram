namespace MyTelegram.Domain.Events.User;

public class UserGlobalPrivacySettingsChangedEvent(RequestInfo requestInfo, GlobalPrivacySettings globalPrivacySettings)
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public GlobalPrivacySettings GlobalPrivacySettings { get; } = globalPrivacySettings;
}