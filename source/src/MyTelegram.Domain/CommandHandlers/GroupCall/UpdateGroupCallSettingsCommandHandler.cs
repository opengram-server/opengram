using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;

namespace MyTelegram.Domain.CommandHandlers.GroupCall;

public class UpdateGroupCallSettingsCommandHandler : CommandHandler<GroupCallAggregate, GroupCallId, UpdateGroupCallSettingsCommand>
{
    public override Task ExecuteAsync(GroupCallAggregate aggregate,
        UpdateGroupCallSettingsCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateSettings(
            command.RequestInfo,
            command.JoinMuted);

        return Task.CompletedTask;
    }
}
