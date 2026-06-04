using MyTelegram.Domain.Aggregates.UserPassword;
using MyTelegram.Domain.Commands.UserPassword;

namespace MyTelegram.Domain.CommandHandlers.UserPassword;

public class CreateSrpSessionCommandHandler : CommandHandler<UserPasswordAggregate, UserPasswordId, CreateSrpSessionCommand>
{
    public override Task ExecuteAsync(UserPasswordAggregate aggregate, CreateSrpSessionCommand command, CancellationToken cancellationToken)
    {
        aggregate.CreateSrpSession(
            command.UserId,
            command.SrpId,
            command.SrpB,
            command.SrpBPrivate,
            command.RequestInfo);

        return Task.CompletedTask;
    }
}
