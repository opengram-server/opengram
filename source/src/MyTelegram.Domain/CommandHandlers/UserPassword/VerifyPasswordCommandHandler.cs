using MyTelegram.Domain.Aggregates.UserPassword;
using MyTelegram.Domain.Commands.UserPassword;

namespace MyTelegram.Domain.CommandHandlers.UserPassword;

public class VerifyPasswordCommandHandler : CommandHandler<UserPasswordAggregate, UserPasswordId, VerifyPasswordCommand>
{
    public override Task ExecuteAsync(UserPasswordAggregate aggregate, VerifyPasswordCommand command, CancellationToken cancellationToken)
    {
        aggregate.VerifyPassword(
            command.UserId,
            command.SrpId,
            command.RequestInfo);

        return Task.CompletedTask;
    }
}
