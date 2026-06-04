using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;

namespace MyTelegram.Domain.CommandHandlers.GroupCall;

public class ToggleGroupCallRecordCommandHandler : CommandHandler<GroupCallAggregate, GroupCallId, ToggleGroupCallRecordCommand>
{
    public override Task ExecuteAsync(GroupCallAggregate aggregate,
        ToggleGroupCallRecordCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ToggleRecording(
            command.RequestInfo,
            command.Start,
            command.Video,
            command.Title,
            command.VideoPortrait,
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        return Task.CompletedTask;
    }
}
