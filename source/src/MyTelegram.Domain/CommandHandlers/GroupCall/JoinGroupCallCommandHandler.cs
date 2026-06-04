using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;

namespace MyTelegram.Domain.CommandHandlers.GroupCall;

public class JoinGroupCallCommandHandler : CommandHandler<GroupCallAggregate, GroupCallId, JoinGroupCallCommand>
{
    public override Task ExecuteAsync(GroupCallAggregate aggregate,
        JoinGroupCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.JoinGroupCall(
            command.RequestInfo,
            command.PeerId,
            command.PeerType,
            command.Source,
            command.Muted,
            command.VideoStopped,
            command.Params,
            command.Date);

        return Task.CompletedTask;
    }
}
