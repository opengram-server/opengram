using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Checklists;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Domain.Aggregates.Channel;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using MyTelegram.Queries;
using MyTelegram.Domain.Aggregates.Checklist;
using MyTelegram.Domain.Commands.Checklist;
using MyTelegram;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Interface for managing checklist functionality
/// </summary>
public interface IChecklistAppService
{
    Task<MyTelegram.Domain.Shared.Checklists.Checklist> CreateChecklistAsync(CreateChecklistRequest request);
    Task<MyTelegram.Domain.Shared.Checklists.Checklist?> GetChecklistAsync(string checklistId);
    Task<List<MyTelegram.Domain.Shared.Checklists.Checklist>> GetChannelChecklistsAsync(long channelId, int offset = 0, int limit = 50);
    Task<List<MyTelegram.Domain.Shared.Checklists.Checklist>> GetUserChecklistsAsync(long userId, int offset = 0, int limit = 50);
    Task<bool> UpdateChecklistAsync(UpdateChecklistRequest request);
    Task<bool> ToggleTaskCompletionAsync(ToggleChecklistTaskRequest request);
    Task<bool> DeleteChecklistAsync(string checklistId, long deletedBy);
    Task<ChecklistStatistics> GetChecklistStatisticsAsync(long channelId, DateTime? from = null, DateTime? to = null);
    Task<bool> AddTasksToChecklistAsync(string checklistId, List<InputChecklistTask> tasks, long addedBy);
    Task<bool> RemoveTasksFromChecklistAsync(string checklistId, List<string> taskIds, long removedBy);
    Task<bool> MarkChecklistAsCompletedAsync(string checklistId, long completedBy);
}

/// <summary>
/// Service for managing checklist functionality
/// </summary>
public sealed class ChecklistAppService(
    ILogger<ChecklistAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IUserAppService userAppService) : IChecklistAppService
{
    public async Task<MyTelegram.Domain.Shared.Checklists.Checklist> CreateChecklistAsync(CreateChecklistRequest request)
    {
        logger.LogInformation("Creating checklist '{Title}' by user {UserId} in peer {PeerId}", 
            request.Checklist.Title, request.SenderId, request.PeerId);

        // Validate checklist
        var validationResult = await ValidateChecklistAsync(request.Checklist);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(validationResult.ErrorMessage);
        }

        var checklistId = $"checklist-{Guid.NewGuid()}";
        // In a real implementation, we might want to use a deterministic ID based on message ID if possible, 
        // but for now a GUID is fine for the Aggregate ID.
        
        var tasks = request.Checklist.Tasks.Select((t, i) => new MyTelegram.Domain.Shared.Checklists.ChecklistTask
        {
            Id = Guid.NewGuid().ToString(),
            ChecklistId = checklistId,
            Text = t.Text,
            Entities = t.Entities,
            IsMandatory = t.IsMandatory,
            Position = i,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        var command = new CreateChecklistCommand(
            new ChecklistId(checklistId),
            new MyTelegram.RequestInfo(
                ReqMsgId: 0,
                UserId: request.SenderId,
                AccessHashKeyId: 0,
                AuthKeyId: 0,
                PermAuthKeyId: 0,
                RequestId: Guid.NewGuid(),
                Layer: 0,
                Date: DateTime.UtcNow.Ticks,
                DeviceType: DeviceType.Desktop,
                AddRequestIdToCache: true,
                IsSubRequest: false
            ),
            request.SenderId,
            request.PeerId,
            request.Checklist.Title,
            request.Checklist.TitleEntities,
            tasks
        );

        await commandBus.PublishAsync(command, CancellationToken.None);

        // Construct return object (in a real CQRS system, we might query the read model, 
        // but here we construct it to return immediately)
        var checklist = new MyTelegram.Domain.Shared.Checklists.Checklist
        {
            Id = checklistId,
            SenderId = request.SenderId,
            Title = request.Checklist.Title,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Tasks = tasks,
            Status = MyTelegram.Domain.Shared.Checklists.ChecklistStatus.Active
        };

        logger.LogInformation("Checklist '{Title}' created successfully with {TaskCount} tasks", 
            request.Checklist.Title, tasks.Count);

        return checklist;
    }

    public async Task<MyTelegram.Domain.Shared.Checklists.Checklist?> GetChecklistAsync(string checklistId)
    {
        // This should query the ReadModel
        // For now, returning null as we haven't implemented the QueryHandler for ChecklistReadModel yet
        // In a full implementation, we would have:
        // var readModel = await queryProcessor.ProcessAsync(new GetChecklistQuery(checklistId));
        // return MapToDto(readModel);
        return null; 
    }

    public async Task<List<MyTelegram.Domain.Shared.Checklists.Checklist>> GetChannelChecklistsAsync(long channelId, int offset = 0, int limit = 50)
    {
        // Placeholder for query
        return new List<MyTelegram.Domain.Shared.Checklists.Checklist>();
    }

    public async Task<List<MyTelegram.Domain.Shared.Checklists.Checklist>> GetUserChecklistsAsync(long userId, int offset = 0, int limit = 50)
    {
        // Placeholder for query
        return new List<MyTelegram.Domain.Shared.Checklists.Checklist>();
    }

    public async Task<bool> UpdateChecklistAsync(UpdateChecklistRequest request)
    {
        logger.LogInformation("Updating checklist {ChecklistId} by user {UserId}", 
            request.ChecklistId, request.UpdatedBy);

        var checklist = await GetChecklistAsync(request.ChecklistId);
        if (checklist == null)
        {
            return false;
        }

        // Check if user has permission to update
        if (!await CanUpdateChecklistAsync(request.UpdatedBy, checklist))
        {
            throw new UnauthorizedAccessException("User doesn't have permission to update this checklist");
        }

        var command = new UpdateChecklistCommand(
            new ChecklistId(request.ChecklistId),
            new MyTelegram.RequestInfo(
                ReqMsgId: 0,
                UserId: request.UpdatedBy,
                AccessHashKeyId: 0,
                AuthKeyId: 0,
                PermAuthKeyId: 0,
                RequestId: Guid.NewGuid(),
                Layer: 0,
                Date: DateTime.UtcNow.Ticks,
                DeviceType: DeviceType.Desktop,
                AddRequestIdToCache: true,
                IsSubRequest: false
            ),
            request.UpdatedBy,
            request.Title,
            request.TitleEntities
        );

        await commandBus.PublishAsync(command, CancellationToken.None);

        // Handle task updates (add/remove) separately if needed, or extend UpdateChecklistCommand
        // For now, assuming title update is the main "UpdateChecklist" action
        // Task additions/removals are handled by specific methods

        if (request.NewTasks?.Any() == true)
        {
            await AddTasksToChecklistAsync(request.ChecklistId, request.NewTasks, request.UpdatedBy);
        }

        if (request.TaskIdsToRemove?.Any() == true)
        {
            await RemoveTasksFromChecklistAsync(request.ChecklistId, request.TaskIdsToRemove, request.UpdatedBy);
        }

        logger.LogInformation("Checklist {ChecklistId} updated successfully", request.ChecklistId);
        return true;
    }

    public async Task<bool> ToggleTaskCompletionAsync(ToggleChecklistTaskRequest request)
    {
        var command = new ToggleChecklistTaskCommand(
            new ChecklistId(request.ChecklistId),
            new MyTelegram.RequestInfo(
                ReqMsgId: 0,
                UserId: request.UserId,
                AccessHashKeyId: 0,
                AuthKeyId: 0,
                PermAuthKeyId: 0,
                RequestId: Guid.NewGuid(),
                Layer: 0,
                Date: DateTime.UtcNow.Ticks,
                DeviceType: DeviceType.Desktop,
                AddRequestIdToCache: true,
                IsSubRequest: false
            ),
            request.TaskId,
            request.UserId,
            request.MarkCompleted
        );

        await commandBus.PublishAsync(command, CancellationToken.None);
        return true;
    }

    public async Task<bool> DeleteChecklistAsync(string checklistId, long deletedBy)
    {
        logger.LogInformation("Deleting checklist {ChecklistId} by user {UserId}", checklistId, deletedBy);

        var checklist = await GetChecklistAsync(checklistId);
        if (checklist == null)
        {
            return false;
        }

        // Check if user has permission to delete
        if (!await CanDeleteChecklistAsync(deletedBy, checklist))
        {
            throw new UnauthorizedAccessException("User doesn't have permission to delete this checklist");
        }

        var command = new DeleteChecklistCommand(
            new ChecklistId(checklistId),
            new MyTelegram.RequestInfo(
                ReqMsgId: 0,
                UserId: deletedBy,
                AccessHashKeyId: 0,
                AuthKeyId: 0,
                PermAuthKeyId: 0,
                RequestId: Guid.NewGuid(),
                Layer: 0,
                Date: DateTime.UtcNow.Ticks,
                DeviceType: DeviceType.Desktop,
                AddRequestIdToCache: true,
                IsSubRequest: false
            ),
            deletedBy
        );

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Checklist {ChecklistId} deleted successfully", checklistId);
        return true;
    }

    public async Task<ChecklistStatistics> GetChecklistStatisticsAsync(long channelId, DateTime? from = null, DateTime? to = null)
    {
        return new ChecklistStatistics { ChannelId = channelId };
    }

    public async Task<bool> AddTasksToChecklistAsync(string checklistId, List<InputChecklistTask> tasks, long addedBy)
    {
        var newTasks = tasks.Select((t, i) => new MyTelegram.Domain.Shared.Checklists.ChecklistTask
        {
            Id = Guid.NewGuid().ToString(),
            ChecklistId = checklistId,
            Text = t.Text,
            Entities = t.Entities,
            IsMandatory = t.IsMandatory,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        var command = new AddChecklistTasksCommand(
            new ChecklistId(checklistId),
            new MyTelegram.RequestInfo(
                ReqMsgId: 0,
                UserId: addedBy,
                AccessHashKeyId: 0,
                AuthKeyId: 0,
                PermAuthKeyId: 0,
                RequestId: Guid.NewGuid(),
                Layer: 0,
                Date: DateTime.UtcNow.Ticks,
                DeviceType: DeviceType.Desktop,
                AddRequestIdToCache: true,
                IsSubRequest: false
            ),
            newTasks,
            addedBy
        );

        await commandBus.PublishAsync(command, CancellationToken.None);
        return true;
    }

    public async Task<bool> RemoveTasksFromChecklistAsync(string checklistId, List<string> taskIds, long removedBy)
    {
        return true;
    }

    public async Task<bool> MarkChecklistAsCompletedAsync(string checklistId, long completedBy)
    {
        return true;
    }

    private async Task<ChecklistValidationResult> ValidateChecklistAsync(InputChecklist checklist)
    {
        var result = new ChecklistValidationResult { IsValid = true };
        if (string.IsNullOrWhiteSpace(checklist.Title))
        {
            result.IsValid = false;
            result.ErrorMessage = "Title is required";
        }
        return result;
    }

    private async Task<bool> CanUpdateChecklistAsync(long userId, MyTelegram.Domain.Shared.Checklists.Checklist checklist)
    {
        // Checklist creator can always update
        if (checklist.SenderId == userId)
        {
            return true;
        }

        // Check if user is admin in the channel
        var peerId = await GetPeerIdFromMessageAsync(checklist.MessageId.ToString());
        return await IsChannelAdminAsync(userId, peerId);
    }

    private async Task<bool> CanToggleTaskAsync(long userId, MyTelegram.Domain.Shared.Checklists.Checklist checklist)
    {
        // Any user in the chat can toggle tasks
        // This could be restricted based on channel settings
        return true;
    }

    private async Task<bool> CanDeleteChecklistAsync(long userId, MyTelegram.Domain.Shared.Checklists.Checklist checklist)
    {
        // Checklist creator can always delete
        if (checklist.SenderId == userId)
        {
            return true;
        }

        // Channel admins can delete any checklist
        var peerId = await GetPeerIdFromMessageAsync(checklist.MessageId.ToString());
        return await IsChannelAdminAsync(userId, peerId);
    }

    private async Task<bool> CanCompleteChecklistAsync(long userId, MyTelegram.Domain.Shared.Checklists.Checklist checklist)
    {
        // Checklist creator can always complete
        if (checklist.SenderId == userId)
        {
            return true;
        }

        // Channel admins can complete any checklist
        var peerId = await GetPeerIdFromMessageAsync(checklist.MessageId.ToString());
        return await IsChannelAdminAsync(userId, peerId);
    }

    private async Task<long> GenerateMessageIdAsync(long peerId)
    {
        // Generate unique message ID for the peer
        return await Task.FromResult(DateTime.UtcNow.Ticks);
    }

    private async Task<long> GetPeerIdFromMessageAsync(string messageId)
    {
        // In a real implementation, this would retrieve the peer ID from the message
        return await Task.FromResult(0L);
    }

    private async Task<bool> IsChannelAdminAsync(long userId, long channelId)
    {
        // In a real implementation, this would check channel participant permissions
        return await Task.FromResult(true); // Placeholder
    }

    private async Task SendTaskCompletedMessageAsync(string checklistId, string taskId, long userId)
    {
        var checklist = await GetChecklistAsync(checklistId);
        if (checklist == null) return;
        
        var command = new SendChecklistTaskCompletedCommand(
            new ChannelId(checklist.PeerId.ToString()),
            checklistId,
            taskId,
            userId,
            DateTime.UtcNow
        );

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);
    }

    private async Task SendTasksAddedMessageAsync(string checklistId, List<MyTelegram.Domain.Shared.Checklists.ChecklistTask> tasks, long addedBy)
    {
        var checklist = await GetChecklistAsync(checklistId);
        if (checklist == null) return;
        
        var command = new SendChecklistTasksAddedCommand(
            new ChannelId(checklist.PeerId.ToString()),
            checklistId,
            tasks,
            addedBy,
            DateTime.UtcNow
        );

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);
    }
}

// Query and command classes for checklist operations
// Queries are now in MyTelegram.Queries

