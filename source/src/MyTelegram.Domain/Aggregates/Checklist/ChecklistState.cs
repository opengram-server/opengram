using MyTelegram.Domain.Shared.Checklists;
using MyTelegram.Domain.Events.Checklist;

namespace MyTelegram.Domain.Aggregates.Checklist;

public interface IChecklistState
{
    string Title { get; }
    List<ChecklistTask> Tasks { get; }
    long SenderId { get; }
    long PeerId { get; }
    bool IsDeleted { get; }
}

public class ChecklistState : AggregateState<ChecklistAggregate, ChecklistId, ChecklistState>, IChecklistState,
    IApply<ChecklistCreatedEvent>,
    IApply<ChecklistTaskToggledEvent>,
    IApply<ChecklistTasksAddedEvent>,
    IApply<ChecklistDeletedEvent>,
    IApply<ChecklistUpdatedEvent>
{
    public string Title { get; private set; } = string.Empty;
    public List<ChecklistTask> Tasks { get; private set; } = new();
    public long SenderId { get; private set; }
    public long PeerId { get; private set; }
    public bool IsDeleted { get; private set; }

    public void Apply(ChecklistCreatedEvent aggregateEvent)
    {
        Title = aggregateEvent.Title;
        Tasks = aggregateEvent.Tasks;
        SenderId = aggregateEvent.SenderId;
        PeerId = aggregateEvent.PeerId;
    }

    public void Apply(ChecklistTaskToggledEvent aggregateEvent)
    {
        var task = Tasks.FirstOrDefault(t => t.Id == aggregateEvent.TaskId);
        if (task != null)
        {
            task.IsCompleted = aggregateEvent.IsCompleted;
            task.CompletedBy = aggregateEvent.IsCompleted ? aggregateEvent.UserId : 0;
            task.CompletedAt = aggregateEvent.IsCompleted ? DateTime.UtcNow : null;
        }
    }

    public void Apply(ChecklistTasksAddedEvent aggregateEvent)
    {
        Tasks.AddRange(aggregateEvent.Tasks);
    }

    public void Apply(ChecklistDeletedEvent aggregateEvent)
    {
        IsDeleted = true;
    }

    public void Apply(ChecklistUpdatedEvent aggregateEvent)
    {
        if (aggregateEvent.Title != null)
        {
            Title = aggregateEvent.Title;
        }
    }
}
