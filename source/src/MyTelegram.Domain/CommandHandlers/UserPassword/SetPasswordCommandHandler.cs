using MyTelegram.Domain.Aggregates.UserPassword;
using MyTelegram.Domain.Commands.UserPassword;

namespace MyTelegram.Domain.CommandHandlers.UserPassword;

public class SetPasswordCommandHandler : CommandHandler<UserPasswordAggregate, UserPasswordId, SetPasswordCommand>
{
    public override Task ExecuteAsync(UserPasswordAggregate aggregate, SetPasswordCommand command, CancellationToken cancellationToken)
    {
        aggregate.SetPassword(
            command.UserId,
            command.Salt1,
            command.Salt2,
            command.V,
            command.Hint,
            command.Email,
            command.G,
            command.P,
            command.RequestInfo);

        return Task.CompletedTask;
    }
}
