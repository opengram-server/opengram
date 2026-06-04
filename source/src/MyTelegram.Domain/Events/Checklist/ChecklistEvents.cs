using MyTelegram.Domain.Aggregates.Checklist;
using MyTelegram.Domain.Shared.Checklists;

namespace MyTelegram.Domain.Events.Checklist;

public class ChecklistCreatedEvent : AggregateEvent<ChecklistAggregate, ChecklistId>
{
    public RequestInfo RequestInfo { get; }
    public long SenderId { get; }
    public long PeerId { get; }
    public string Title { get; }
    public List<string> TitleEntities { get; }
    public List<ChecklistTask> Tasks { get; }

    public ChecklistCreatedEvent(RequestInfo requestInfo,
        long senderId,
        long peerId,
        string title,
        List<string> titleEntities,
        List<ChecklistTask> tasks)
    {
        RequestInfo = requestInfo;
        SenderId = senderId;
        PeerId = peerId;
        Title = title;
        TitleEntities = titleEntities;
        Tasks = tasks;
    }
}

public class ChecklistTaskToggledEvent : AggregateEvent<ChecklistAggregate, ChecklistId>
{
    public RequestInfo RequestInfo { get; }
    public string TaskId { get; }
    public long UserId { get; }
    public bool IsCompleted { get; }

    public ChecklistTaskToggledEvent(RequestInfo requestInfo, string taskId, long userId, bool isCompleted)
    {
        RequestInfo = requestInfo;
        TaskId = taskId;
        UserId = userId;
        IsCompleted = isCompleted;
    }
}

public class ChecklistTasksAddedEvent : AggregateEvent<ChecklistAggregate, ChecklistId>
{
    public RequestInfo RequestInfo { get; }
    public List<ChecklistTask> Tasks { get; }
    public long AddedBy { get; }

    public ChecklistTasksAddedEvent(RequestInfo requestInfo, List<ChecklistTask> tasks, long addedBy)
    {
        RequestInfo = requestInfo;
        Tasks = tasks;
        AddedBy = addedBy;
    }
}

public class ChecklistDeletedEvent : AggregateEvent<ChecklistAggregate, ChecklistId>
{
    public RequestInfo RequestInfo { get; }
    public long DeletedBy { get; }

    public ChecklistDeletedEvent(RequestInfo requestInfo, long deletedBy)
    {
        RequestInfo = requestInfo;
        DeletedBy = deletedBy;
    }
}

public class ChecklistUpdatedEvent : AggregateEvent<ChecklistAggregate, ChecklistId>
{
    public RequestInfo RequestInfo { get; }
    public long UpdatedBy { get; }
    public string? Title { get; }
    public List<string>? TitleEntities { get; }

    public ChecklistUpdatedEvent(RequestInfo requestInfo, long updatedBy, string? title, List<string>? titleEntities)
    {
        RequestInfo = requestInfo;
        UpdatedBy = updatedBy;
        Title = title;
        TitleEntities = titleEntities;
    }
}
