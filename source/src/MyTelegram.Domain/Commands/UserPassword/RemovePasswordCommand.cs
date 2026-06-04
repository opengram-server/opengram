using MyTelegram.Domain.Aggregates.UserPassword;

namespace MyTelegram.Domain.Commands.UserPassword;

/// <summary>
/// Command to remove user's 2FA password
/// </summary>
public class RemovePasswordCommand : RequestCommand2<UserPasswordAggregate, UserPasswordId, IExecutionResult>
{
    public RemovePasswordCommand(
        UserPasswordId aggregateId,
        RequestInfo requestInfo,
        long userId) : base(aggregateId, requestInfo)
    {
        UserId = userId;
    }

    public long UserId { get; }
}
