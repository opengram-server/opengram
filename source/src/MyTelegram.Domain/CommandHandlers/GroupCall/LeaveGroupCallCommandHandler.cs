using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;

namespace MyTelegram.Domain.CommandHandlers.GroupCall;

public class LeaveGroupCallCommandHandler : CommandHandler<GroupCallAggregate, GroupCallId, LeaveGroupCallCommand>
{
    public override Task ExecuteAsync(GroupCallAggregate aggregate,
        LeaveGroupCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.LeaveGroupCall(
            command.RequestInfo,
            command.PeerId,
            command.Source,
            command.Date);

        return Task.CompletedTask;
    }
}
