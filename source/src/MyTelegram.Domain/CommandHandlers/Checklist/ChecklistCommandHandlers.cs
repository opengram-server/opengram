using MyTelegram.Domain.Aggregates.Checklist;
using MyTelegram.Domain.Commands.Checklist;

namespace MyTelegram.Domain.CommandHandlers.Checklist;

public class CreateChecklistCommandHandler : CommandHandler<ChecklistAggregate, ChecklistId, CreateChecklistCommand>
{
    public override Task ExecuteAsync(ChecklistAggregate aggregate, CreateChecklistCommand command, CancellationToken cancellationToken)
    {
        aggregate.Create(command.RequestInfo, command.SenderId, command.PeerId, command.Title, command.TitleEntities, command.Tasks);
        return Task.CompletedTask;
    }
}

public class ToggleChecklistTaskCommandHandler : CommandHandler<ChecklistAggregate, ChecklistId, ToggleChecklistTaskCommand>
{
    public override Task ExecuteAsync(ChecklistAggregate aggregate, ToggleChecklistTaskCommand command, CancellationToken cancellationToken)
    {
        aggregate.ToggleTask(command.RequestInfo, command.TaskId, command.UserId, command.IsCompleted);
        return Task.CompletedTask;
    }
}

public class AddChecklistTasksCommandHandler : CommandHandler<ChecklistAggregate, ChecklistId, AddChecklistTasksCommand>
{
    public override Task ExecuteAsync(ChecklistAggregate aggregate, AddChecklistTasksCommand command, CancellationToken cancellationToken)
    {
        aggregate.AddTasks(command.RequestInfo, command.Tasks, command.AddedBy);
        return Task.CompletedTask;
    }
}

public class UpdateChecklistCommandHandler : CommandHandler<ChecklistAggregate, ChecklistId, UpdateChecklistCommand>
{
    public override Task ExecuteAsync(ChecklistAggregate aggregate, UpdateChecklistCommand command, CancellationToken cancellationToken)
    {
        aggregate.Update(command.RequestInfo, command.UpdatedBy, command.Title, command.TitleEntities);
        return Task.CompletedTask;
    }
}

public class DeleteChecklistCommandHandler : CommandHandler<ChecklistAggregate, ChecklistId, DeleteChecklistCommand>
{
    public override Task ExecuteAsync(ChecklistAggregate aggregate, DeleteChecklistCommand command, CancellationToken cancellationToken)
    {
        aggregate.Delete(command.RequestInfo, command.DeletedBy);
        return Task.CompletedTask;
    }
}
