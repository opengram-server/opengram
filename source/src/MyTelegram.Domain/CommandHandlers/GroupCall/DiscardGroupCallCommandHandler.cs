using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;

namespace MyTelegram.Domain.CommandHandlers.GroupCall;

public class DiscardGroupCallCommandHandler : CommandHandler<GroupCallAggregate, GroupCallId, DiscardGroupCallCommand>
{
    public override Task ExecuteAsync(GroupCallAggregate aggregate,
        DiscardGroupCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.DiscardGroupCall(
            command.RequestInfo,
            command.Date);

        return Task.CompletedTask;
    }
}
