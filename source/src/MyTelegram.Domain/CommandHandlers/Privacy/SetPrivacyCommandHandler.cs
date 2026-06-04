using MyTelegram.Domain.Commands.Privacy;

namespace MyTelegram.Domain.CommandHandlers.Privacy;

public class SetPrivacyCommandHandler : CommandHandler<UserAggregate, UserId, SetPrivacyCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, SetPrivacyCommand command, CancellationToken cancellationToken)
    {
        aggregate.SetPrivacyRules(command.RequestInfo, command.PrivacyType, command.Rules);
        return Task.CompletedTask;
    }
}
