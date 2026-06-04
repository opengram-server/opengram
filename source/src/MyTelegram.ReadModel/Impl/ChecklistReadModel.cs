using MyTelegram.Domain.Aggregates.Checklist;
using MyTelegram.Domain.Events.Checklist;
using MyTelegram.Domain.Shared.Checklists;

namespace MyTelegram.ReadModel.Impl;

public class ChecklistReadModel : IChecklistReadModel,
    IAmReadModelFor<ChecklistAggregate, ChecklistId, ChecklistCreatedEvent>,
    IAmReadModelFor<ChecklistAggregate, ChecklistId, ChecklistTaskToggledEvent>,
    IAmReadModelFor<ChecklistAggregate, ChecklistId, ChecklistTasksAddedEvent>,
    IAmReadModelFor<ChecklistAggregate, ChecklistId, ChecklistUpdatedEvent>,
    IAmReadModelFor<ChecklistAggregate, ChecklistId, ChecklistDeletedEvent>
{
    public string Id { get; private set; } = string.Empty;
    public long? Version { get; set; }
    public long SenderId { get; private set; }
    public long PeerId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public List<ChecklistTask> Tasks { get; private set; } = new();
    public bool IsDeleted { get; private set; }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChecklistAggregate, ChecklistId, ChecklistCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        SenderId = domainEvent.AggregateEvent.SenderId;
        PeerId = domainEvent.AggregateEvent.PeerId;
        Title = domainEvent.AggregateEvent.Title;
        Tasks = domainEvent.AggregateEvent.Tasks;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChecklistAggregate, ChecklistId, ChecklistTaskToggledEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var task = Tasks.FirstOrDefault(t => t.Id == domainEvent.AggregateEvent.TaskId);
        if (task != null)
        {
            task.IsCompleted = domainEvent.AggregateEvent.IsCompleted;
            task.CompletedBy = domainEvent.AggregateEvent.IsCompleted ? domainEvent.AggregateEvent.UserId : 0;
            task.CompletedAt = domainEvent.AggregateEvent.IsCompleted ? DateTime.UtcNow : null;
        }
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChecklistAggregate, ChecklistId, ChecklistTasksAddedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Tasks.AddRange(domainEvent.AggregateEvent.Tasks);
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChecklistAggregate, ChecklistId, ChecklistUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.Title != null)
        {
            Title = domainEvent.AggregateEvent.Title;
        }
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<ChecklistAggregate, ChecklistId, ChecklistDeletedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        IsDeleted = true;
        return Task.CompletedTask;
    }
}

public interface IChecklistReadModel : IReadModel
{
    string Id { get; }
    long SenderId { get; }
    long PeerId { get; }
    string Title { get; }
    List<ChecklistTask> Tasks { get; }
    bool IsDeleted { get; }
}
