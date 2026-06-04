using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;

namespace MyTelegram.Domain.CommandHandlers.GroupCall;

public class StartScheduledGroupCallCommandHandler : CommandHandler<GroupCallAggregate, GroupCallId, StartScheduledGroupCallCommand>
{
    public override Task ExecuteAsync(
        GroupCallAggregate aggregate,
        StartScheduledGroupCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.StartScheduledGroupCall(
            command.RequestInfo,
            command.Date);

        return Task.CompletedTask;
    }
}
