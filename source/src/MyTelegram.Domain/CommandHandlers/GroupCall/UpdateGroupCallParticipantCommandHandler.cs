using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;

namespace MyTelegram.Domain.CommandHandlers.GroupCall;

public class UpdateGroupCallParticipantCommandHandler : CommandHandler<GroupCallAggregate, GroupCallId, UpdateGroupCallParticipantCommand>
{
    public override Task ExecuteAsync(GroupCallAggregate aggregate,
        UpdateGroupCallParticipantCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateParticipant(
            command.RequestInfo,
            command.PeerId,
            command.Muted,
            command.MutedByAdmin,
            command.Volume,
            command.RaiseHand,
            command.VideoStopped,
            command.Date);

        return Task.CompletedTask;
    }
}
