using MyTelegram.Domain.Aggregates.UserPassword;
using MyTelegram.Domain.Commands.UserPassword;

namespace MyTelegram.Domain.CommandHandlers.UserPassword;

public class RemovePasswordCommandHandler : CommandHandler<UserPasswordAggregate, UserPasswordId, RemovePasswordCommand>
{
    public override Task ExecuteAsync(UserPasswordAggregate aggregate, RemovePasswordCommand command, CancellationToken cancellationToken)
    {
        aggregate.RemovePassword(command.UserId, command.RequestInfo);
        return Task.CompletedTask;
    }
}
