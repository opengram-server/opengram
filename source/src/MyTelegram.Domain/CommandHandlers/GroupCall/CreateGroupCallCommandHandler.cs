using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;

namespace MyTelegram.Domain.CommandHandlers.GroupCall;

public class CreateGroupCallCommandHandler : CommandHandler<GroupCallAggregate, GroupCallId, CreateGroupCallCommand>
{
    public override Task ExecuteAsync(GroupCallAggregate aggregate,
        CreateGroupCallCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.CreateGroupCall(
            command.RequestInfo,
            command.CallId,
            command.AccessHash,
            command.PeerId,
            command.PeerType,
            command.Title,
            command.RtmpStream,
            command.StreamDcId,
            command.ScheduleDate,
            command.RandomId,
            command.Date);

        return Task.CompletedTask;
    }
}
