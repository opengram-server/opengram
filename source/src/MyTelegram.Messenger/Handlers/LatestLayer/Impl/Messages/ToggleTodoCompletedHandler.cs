// ReSharper disable All

using MyTelegram.Services.Services;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Domain.Shared.Checklists;

namespace MyTelegram.Messenger.Handlers.Messages;

///<summary>
/// Toggle the completion status of one or more checklist/todo items in a message.
/// See <a href="https://corefork.telegram.org/method/messages.toggleTodoCompleted" />
///</summary>
internal sealed class ToggleTodoCompletedHandler(
    IPeerHelper peerHelper,
    IChecklistAppService checklistAppService,
    ILogger<ToggleTodoCompletedHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestToggleTodoCompleted, MyTelegram.Schema.IUpdates>,
    Messages.IToggleTodoCompletedHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestToggleTodoCompleted obj)
    {
        // Resolve peer
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        if (peer.PeerId == 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }

        // Validate message ID
        if (obj.MsgId <= 0)
        {
            throw new RpcException(RpcErrors.RpcErrors400.MsgIdInvalid);
        }

        // Validate that at least one task status change is provided
        if ((obj.Completed == null || obj.Completed.Count == 0) &&
            (obj.Incompleted == null || obj.Incompleted.Count == 0))
        {
            throw new RpcException(RpcErrors.RpcErrors400.MsgIdInvalid);
        }

        // Build checklist ID from peer+message
        var checklistId = $"checklist-{peer.PeerId}-{obj.MsgId}";

        // Toggle completed tasks
        if (obj.Completed is { Count: > 0 })
        {
            foreach (var taskIndex in obj.Completed)
            {
                var request = new ToggleChecklistTaskRequest
                {
                    ChecklistId = checklistId,
                    TaskId = taskIndex.ToString(),
                    UserId = input.UserId,
                    MarkCompleted = true
                };

                await checklistAppService.ToggleTaskCompletionAsync(request);
            }

            logger.LogInformation(
                "ToggleTodoCompleted: Marked {Count} tasks as completed, ChecklistId={ChecklistId}, UserId={UserId}",
                obj.Completed.Count, checklistId, input.UserId);
        }

        // Toggle incompleted tasks
        if (obj.Incompleted is { Count: > 0 })
        {
            foreach (var taskIndex in obj.Incompleted)
            {
                var request = new ToggleChecklistTaskRequest
                {
                    ChecklistId = checklistId,
                    TaskId = taskIndex.ToString(),
                    UserId = input.UserId,
                    MarkCompleted = false
                };

                await checklistAppService.ToggleTaskCompletionAsync(request);
            }

            logger.LogInformation(
                "ToggleTodoCompleted: Marked {Count} tasks as incompleted, ChecklistId={ChecklistId}, UserId={UserId}",
                obj.Incompleted.Count, checklistId, input.UserId);
        }

        return new TUpdates
        {
            Users = [],
            Updates = [],
            Chats = [],
            Date = CurrentDate
        };
    }
}
