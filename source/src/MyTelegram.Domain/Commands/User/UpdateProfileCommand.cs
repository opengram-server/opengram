namespace MyTelegram.Domain.Commands.User;

public class UpdateProfileCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    string? firstName,
    string? lastName,
    string? about)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public string? About { get; } = about;
    public string? FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
}
