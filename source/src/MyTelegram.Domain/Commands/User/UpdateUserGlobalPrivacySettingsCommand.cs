namespace MyTelegram.Domain.Commands.User;

public class UpdateUserGlobalPrivacySettingsCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    GlobalPrivacySettings globalPrivacySettings)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public GlobalPrivacySettings GlobalPrivacySettings { get; } = globalPrivacySettings;
}