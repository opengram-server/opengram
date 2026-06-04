namespace MyTelegram.Domain.Commands.Privacy;

public class SetPrivacyCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    PrivacyType privacyType,
    IReadOnlyList<PrivacyValueData> rules)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public PrivacyType PrivacyType { get; } = privacyType;
    public IReadOnlyList<PrivacyValueData> Rules { get; } = rules;
}
