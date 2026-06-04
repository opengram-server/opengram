using MyTelegram.Domain.Aggregates.Checklist;
using MyTelegram.Domain.Shared.Checklists;
using MyTelegram;

using MyTelegram.Domain.Aggregates.Channel;

namespace MyTelegram.Domain.Commands.Checklist;

public class CreateChecklistCommand : Command<ChecklistAggregate, ChecklistId, IExecutionResult>
{
    public RequestInfo RequestInfo { get; }
    public long SenderId { get; }
    public long PeerId { get; }
    public string Title { get; }
    public List<string> TitleEntities { get; }
    public List<ChecklistTask> Tasks { get; }

    public CreateChecklistCommand(ChecklistId aggregateId,
        RequestInfo requestInfo,
        long senderId,
        long peerId,
        string title,
        List<string> titleEntities,
        List<ChecklistTask> tasks) : base(aggregateId)
    {
        RequestInfo = requestInfo;
        SenderId = senderId;
        PeerId = peerId;
        Title = title;
        TitleEntities = titleEntities;
        Tasks = tasks;
    }
}

public class ToggleChecklistTaskCommand : Command<ChecklistAggregate, ChecklistId, IExecutionResult>
{
    public RequestInfo RequestInfo { get; }
    public string TaskId { get; }
    public long UserId { get; }
    public bool IsCompleted { get; }

    public ToggleChecklistTaskCommand(ChecklistId aggregateId,
        RequestInfo requestInfo,
        string taskId,
        long userId,
        bool isCompleted) : base(aggregateId)
    {
        RequestInfo = requestInfo;
        TaskId = taskId;
        UserId = userId;
        IsCompleted = isCompleted;
    }
}

public class AddChecklistTasksCommand : Command<ChecklistAggregate, ChecklistId, IExecutionResult>
{
    public RequestInfo RequestInfo { get; }
    public List<ChecklistTask> Tasks { get; }
    public long AddedBy { get; }

    public AddChecklistTasksCommand(ChecklistId aggregateId,
        RequestInfo requestInfo,
        List<ChecklistTask> tasks,
        long addedBy) : base(aggregateId)
    {
        RequestInfo = requestInfo;
        Tasks = tasks;
        AddedBy = addedBy;
    }
}

public class UpdateChecklistCommand : Command<ChecklistAggregate, ChecklistId, IExecutionResult>
{
    public RequestInfo RequestInfo { get; }
    public long UpdatedBy { get; }
    public string? Title { get; }
    public List<string>? TitleEntities { get; }

    public UpdateChecklistCommand(ChecklistId aggregateId,
        RequestInfo requestInfo,
        long updatedBy,
        string? title,
        List<string>? titleEntities) : base(aggregateId)
    {
        RequestInfo = requestInfo;
        UpdatedBy = updatedBy;
        Title = title;
        TitleEntities = titleEntities;
    }
}

public class DeleteChecklistCommand : Command<ChecklistAggregate, ChecklistId, IExecutionResult>
{
    public RequestInfo RequestInfo { get; }
    public long DeletedBy { get; }

    public DeleteChecklistCommand(ChecklistId aggregateId,
        RequestInfo requestInfo,
        long deletedBy) : base(aggregateId)
    {
        RequestInfo = requestInfo;
        DeletedBy = deletedBy;
    }
}

public class RemoveChecklistTasksCommand : Command<ChecklistAggregate, ChecklistId, IExecutionResult>
{
    public RequestInfo RequestInfo { get; }
    public List<string> TaskIds { get; }
    public long RemovedBy { get; }

    public RemoveChecklistTasksCommand(ChecklistId aggregateId,
        RequestInfo requestInfo,
        List<string> taskIds,
        long removedBy) : base(aggregateId)
    {
        RequestInfo = requestInfo;
        TaskIds = taskIds;
        RemovedBy = removedBy;
    }
}

public class CompleteChecklistCommand : Command<ChecklistAggregate, ChecklistId, IExecutionResult>
{
    public RequestInfo RequestInfo { get; }
    public long CompletedBy { get; }

    public CompleteChecklistCommand(ChecklistId aggregateId,
        RequestInfo requestInfo,
        long completedBy) : base(aggregateId)
    {
        RequestInfo = requestInfo;
        CompletedBy = completedBy;
    }
}

public class SendChecklistTaskCompletedCommand : Command<ChannelAggregate, ChannelId, IExecutionResult>
{
    public string ChecklistId { get; }
    public string TaskId { get; }
    public long UserId { get; }
    public DateTime SentAt { get; }

    public SendChecklistTaskCompletedCommand(ChannelId aggregateId,
        string checklistId,
        string taskId,
        long userId,
        DateTime sentAt) : base(aggregateId)
    {
        ChecklistId = checklistId;
        TaskId = taskId;
        UserId = userId;
        SentAt = sentAt;
    }
}

public class SendChecklistTasksAddedCommand : Command<ChannelAggregate, ChannelId, IExecutionResult>
{
    public string ChecklistId { get; }
    public List<ChecklistTask> AddedTasks { get; }
    public long AddedBy { get; }
    public DateTime SentAt { get; }

    public SendChecklistTasksAddedCommand(ChannelId aggregateId,
        string checklistId,
        List<ChecklistTask> addedTasks,
        long addedBy,
        DateTime sentAt) : base(aggregateId)
    {
        ChecklistId = checklistId;
        AddedTasks = addedTasks;
        AddedBy = addedBy;
        SentAt = sentAt;
    }
}
