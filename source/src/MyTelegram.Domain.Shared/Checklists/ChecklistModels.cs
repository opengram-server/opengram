using System;
using System.Collections.Generic;

namespace MyTelegram.Domain.Shared.Checklists;

/// <summary>
/// Checklist that can be sent as a message
/// </summary>
public class Checklist
{
    public string Id { get; set; } = string.Empty;
    public long MessageId { get; set; }
    public long SenderId { get; set; }
    public long PeerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ChecklistTask> Tasks { get; set; } = new();
    public ChecklistStatus Status { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long CompletedBy { get; set; }
    public int TotalTasks => Tasks.Count;
    public int CompletedTasks => Tasks.Count(t => t.IsCompleted);
    public double CompletionPercentage => TotalTasks > 0 ? (double)CompletedTasks / TotalTasks * 100 : 0;
}

/// <summary>
/// Individual task within a checklist
/// </summary>
public class ChecklistTask
{
    public string Id { get; set; } = string.Empty;
    public string ChecklistId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long CompletedBy { get; set; }
    public List<string> Entities { get; set; } = new(); // Changed from MessageEntity to string
    public int Position { get; set; }
    public bool IsMandatory { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Checklist status
/// </summary>
public enum ChecklistStatus
{
    Draft,
    Active,
    Completed,
    Archived
}

/// <summary>
/// Input model for creating a checklist
/// </summary>
public class InputChecklist
{
    public string Title { get; set; } = string.Empty;
    public List<InputChecklistTask> Tasks { get; set; } = new();
    public List<string> TitleEntities { get; set; } = new(); // Changed from MessageEntity to string
}

/// <summary>
/// Input model for creating a checklist task
/// </summary>
public class InputChecklistTask
{
    public string Text { get; set; } = string.Empty;
    public List<string> Entities { get; set; } = new(); // Changed from MessageEntity to string
    public bool IsMandatory { get; set; }
}

/// <summary>
/// Service message about checklist tasks marked as done or not done
/// </summary>
public class ChecklistTasksDone
{
    public string MessageId { get; set; } = string.Empty;
    public string ChecklistId { get; set; } = string.Empty;
    public long UserId { get; set; }
    public List<string> TaskIds { get; set; } = new();
    public DateTime CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
}

/// <summary>
/// Service message about tasks added to a checklist
/// </summary>
public class ChecklistTasksAdded
{
    public string MessageId { get; set; } = string.Empty;
    public string ChecklistId { get; set; } = string.Empty;
    public long AddedBy { get; set; }
    public List<ChecklistTask> AddedTasks { get; set; } = new();
    public DateTime AddedAt { get; set; }
}

/// <summary>
/// Checklist statistics
/// </summary>
public class ChecklistStatistics
{
    public long ChannelId { get; set; }
    public int TotalChecklists { get; set; }
    public int ActiveChecklists { get; set; }
    public int CompletedChecklists { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double OverallCompletionRate { get; set; }
    public List<UserChecklistStats> UserStats { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// User-specific checklist statistics
/// </summary>
public class UserChecklistStats
{
    public long UserId { get; set; }
    public int ChecklistsCreated { get; set; }
    public int ChecklistsCompleted { get; set; }
    public int TasksCompleted { get; set; }
    public DateTime LastActivity { get; set; }
}

/// <summary>
/// Checklist creation request
/// </summary>
public class CreateChecklistRequest
{
    public long SenderId { get; set; }
    public long PeerId { get; set; }
    public InputChecklist Checklist { get; set; } = new();
    public bool Silent { get; set; }
    public long? ReplyToMessageId { get; set; }
}

/// <summary>
/// Checklist update request
/// </summary>
public class UpdateChecklistRequest
{
    public string ChecklistId { get; set; } = string.Empty;
    public long UpdatedBy { get; set; }
    public string? Title { get; set; }
    public List<string>? TitleEntities { get; set; } // Changed from MessageEntity to string
    public List<InputChecklistTask>? NewTasks { get; set; }
    public List<string>? TaskIdsToRemove { get; set; }
}

/// <summary>
/// Checklist task completion request
/// </summary>
public class ToggleChecklistTaskRequest
{
    public string ChecklistId { get; set; } = string.Empty;
    public string TaskId { get; set; } = string.Empty;
    public long UserId { get; set; }
    public bool MarkCompleted { get; set; }
}

/// <summary>
/// Checklist validation result
/// </summary>
public class ChecklistValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}
