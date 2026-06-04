using MyTelegram.Domain.Events.Checklist;
using MyTelegram.Domain.Shared.Checklists;

namespace MyTelegram.Domain.Aggregates.Checklist;

public class ChecklistAggregate : AggregateRoot<ChecklistAggregate, ChecklistId>
{
    private readonly ChecklistState _state = new();

    public ChecklistAggregate(ChecklistId id) : base(id)
    {
        Register(_state);
    }

    public void Create(RequestInfo requestInfo,
        long senderId,
        long peerId,
        string title,
        List<string> titleEntities,
        List<ChecklistTask> tasks)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new ChecklistCreatedEvent(requestInfo, senderId, peerId, title, titleEntities, tasks));
    }

    public void ToggleTask(RequestInfo requestInfo, string taskId, long userId, bool isCompleted)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        if (!_state.Tasks.Any(t => t.Id == taskId))
        {
            // Task not found
            return;
        }
        
        Emit(new ChecklistTaskToggledEvent(requestInfo, taskId, userId, isCompleted));
    }

    public void AddTasks(RequestInfo requestInfo, List<ChecklistTask> tasks, long addedBy)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new ChecklistTasksAddedEvent(requestInfo, tasks, addedBy));
    }

    public void Delete(RequestInfo requestInfo, long deletedBy)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new ChecklistDeletedEvent(requestInfo, deletedBy));
    }

    public void Update(RequestInfo requestInfo, long updatedBy, string? title, List<string>? titleEntities)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new ChecklistUpdatedEvent(requestInfo, updatedBy, title, titleEntities));
    }
}
