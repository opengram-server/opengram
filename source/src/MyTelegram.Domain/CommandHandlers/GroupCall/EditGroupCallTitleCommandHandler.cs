using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;

namespace MyTelegram.Domain.CommandHandlers.GroupCall;

public class EditGroupCallTitleCommandHandler : CommandHandler<GroupCallAggregate, GroupCallId, EditGroupCallTitleCommand>
{
    public override Task ExecuteAsync(GroupCallAggregate aggregate,
        EditGroupCallTitleCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.EditTitle(
            command.RequestInfo,
            command.Title,
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        return Task.CompletedTask;
    }
}
