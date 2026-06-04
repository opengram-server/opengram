namespace MyTelegram.Domain.Commands.User;

public class UpdateAboutCommand(UserId aggregateId, string? about) : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public string? About { get; } = about;
}