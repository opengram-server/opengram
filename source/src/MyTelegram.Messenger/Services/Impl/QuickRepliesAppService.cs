using Microsoft.Extensions.Logging;
using MyTelegram.Domain;
using MyTelegram.Domain.Shared.QuickReply;
using MyTelegram.Domain.Commands.QuickReply;
using MyTelegram.Schema;
using MyTelegram.Messenger.Services;
using MyTelegram.Messenger.Services.Interfaces;
using MyTelegram.Domain.Shared;
using MyTelegram.Domain.Shared.Business;
using EventFlow.Commands;
using EventFlow.Aggregates.ExecutionResults;
using System.Linq;
using MyTelegram.Domain.Aggregates.QuickReply;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Messenger.Services.Impl;

public class QuickRepliesAppService(
    ILogger<QuickRepliesAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IMessageAppService messageAppService) : IQuickRepliesAppService, ITransientDependency
{
    public async Task<IQuickReplies> GetQuickRepliesAsync(long userId, long hash = 0)
    {
        logger.LogDebug("Getting quick replies for user {UserId} with hash {Hash}", userId, hash);

        var quickReplyReadModel = (await queryProcessor.ProcessAsync(new GetQuickRepliesByUserIdQuery(userId))).FirstOrDefault();
        
        // TODO: Implement hash check
        // if (quickReplies != null && quickReplies.Hash == hash.ToString()) ...

        var shortcuts = quickReplyReadModel?.Shortcuts ?? new List<QuickReplyShortcutItem>();
        
        var tQuickReplies = new TQuickReplies
        {
            QuickReplies = new TVector<IQuickReply>(shortcuts.Select(qr => new TQuickReply
            {
                ShortcutId = qr.ShortcutId,
                Shortcut = qr.Shortcut,
                TopMessage = 0, // TODO: Get top message ID
                Count = 0 // TODO: Get count
            }).ToArray()),
            
            Messages = new TVector<IMessage>(),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };

        // Load messages and related data
        // await LoadQuickReplyDataAsync(userId, shortcuts, tQuickReplies);

        return tQuickReplies;
    }

    public async Task<IMessages> GetQuickReplyMessagesAsync(long userId, int shortcutId, List<int>? messageIds = null, long hash = 0)
    {
        logger.LogDebug("Getting quick reply messages for user {UserId}, shortcut {ShortcutId}", userId, shortcutId);

        // var messages = await queryProcessor.ProcessAsync(new GetQuickReplyMessagesQuery(userId, shortcutId, messageIds ?? new List<int>()));
        
        var result = new TMessages
        {
            Messages = new TVector<IMessage>(),
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };

        return result;
    }

    public async Task<int> CreateQuickReplyShortcutAsync(long userId, string shortcut, int shortcutId = 0)
    {
        logger.LogInformation("Creating quick reply shortcut '{Shortcut}' for user {UserId}", shortcut, userId);

        if (string.IsNullOrEmpty(shortcut) || shortcut.Length > 32)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ShortcutInvalid);
        }

        var quickReplyReadModel = (await queryProcessor.ProcessAsync(new GetQuickRepliesByUserIdQuery(userId))).FirstOrDefault();
        if (quickReplyReadModel?.Shortcuts.Any(qr => qr.Shortcut.Equals(shortcut, StringComparison.OrdinalIgnoreCase)) == true)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ShortNameOccupied);
        }

        if (shortcutId == 0)
        {
            shortcutId = GenerateShortcutId(quickReplyReadModel?.Shortcuts);
        }
        else
        {
            if (quickReplyReadModel?.Shortcuts.Any(qr => qr.ShortcutId == shortcutId) == true)
            {
                 throw new RpcException(RpcErrors.RpcErrors400.ShortcutInvalid); // Or ShortNameOccupied?
            }
        }

        var command = new CreateQuickReplyShortcutCommand(
            QuickReplyId.Create(userId),
            new MyTelegram.RequestInfo(0, userId, 0, 0, 0, Guid.NewGuid(), 0, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            shortcutId,
            shortcut);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Quick reply shortcut '{Shortcut}' created with ID {ShortcutId} for user {UserId}", 
            shortcut, shortcutId, userId);

        return shortcutId;
    }

    public async Task EditQuickReplyShortcutAsync(long userId, int shortcutId, string newShortcut)
    {
        logger.LogInformation("Editing quick reply shortcut {ShortcutId} to '{NewShortcut}' for user {UserId}", 
            shortcutId, newShortcut, userId);

        var quickReplyReadModel = (await queryProcessor.ProcessAsync(new GetQuickRepliesByUserIdQuery(userId))).FirstOrDefault();
        if (quickReplyReadModel?.Shortcuts.All(qr => qr.ShortcutId != shortcutId) == true)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ShortcutInvalid);
        }

        if (string.IsNullOrEmpty(newShortcut) || newShortcut.Length > 32)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ShortcutInvalid);
        }

        if (quickReplyReadModel?.Shortcuts.Any(qr => qr.ShortcutId != shortcutId && qr.Shortcut.Equals(newShortcut, StringComparison.OrdinalIgnoreCase)) == true)
        {
            throw new RpcException(RpcErrors.RpcErrors400.ShortNameOccupied);
        }

        var command = new UpdateQuickReplyShortcutCommand(
            QuickReplyId.Create(userId),
            new MyTelegram.RequestInfo(0, userId, 0, 0, 0, Guid.NewGuid(), 0, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            shortcutId,
            newShortcut);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Quick reply shortcut {ShortcutId} edited successfully for user {UserId}", shortcutId, userId);
    }

    public async Task DeleteQuickReplyShortcutAsync(long userId, int shortcutId)
    {
        logger.LogInformation("Deleting quick reply shortcut {ShortcutId} for user {UserId}", shortcutId, userId);

        var quickReplyReadModel = (await queryProcessor.ProcessAsync(new GetQuickRepliesByUserIdQuery(userId))).FirstOrDefault();
        if (quickReplyReadModel?.Shortcuts.All(qr => qr.ShortcutId != shortcutId) == true)
        {
            throw new RpcException(new RpcError(400, "SHORTCUT_INVALID"));
        }

        var command = new DeleteQuickReplyShortcutCommand(
            QuickReplyId.Create(userId),
            new MyTelegram.RequestInfo(0, userId, 0, 0, 0, Guid.NewGuid(), 0, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            shortcutId);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Quick reply shortcut {ShortcutId} deleted successfully for user {UserId}", shortcutId, userId);
    }

    public async Task<List<long>> SendQuickReplyMessagesAsync(long userId, long peerId, int shortcutId)
    {
        // TODO: Implement sending messages
        return new List<long>();
    }

    public async Task ReorderQuickRepliesAsync(long userId, List<int> order)
    {
        logger.LogInformation("Reordering quick replies for user {UserId}", userId);

        var quickReplyReadModel = (await queryProcessor.ProcessAsync(new GetQuickRepliesByUserIdQuery(userId))).FirstOrDefault();
        var existingShortcutIds = quickReplyReadModel?.Shortcuts.Select(qr => qr.ShortcutId).ToHashSet() ?? new HashSet<int>();

        if (order.Any(id => !existingShortcutIds.Contains(id)))
        {
            throw new RpcException(RpcErrors.RpcErrors400.ShortcutInvalid);
        }

        var command = new ReorderQuickRepliesCommand(
            QuickReplyId.Create(userId),
            new MyTelegram.RequestInfo(0, userId, 0, 0, 0, Guid.NewGuid(), 0, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            order);

        await commandBus.PublishAsync(command, CancellationToken.None);

        logger.LogInformation("Quick replies reordered successfully for user {UserId}", userId);
    }

    public async Task DeleteQuickReplyMessagesAsync(long userId, int shortcutId, List<int> messageIds)
    {
        // TODO: Implement deleting messages
    }

    public async Task<bool> CheckQuickReplyShortcutAsync(long userId, string shortcut)
    {
        var quickReplyReadModel = (await queryProcessor.ProcessAsync(new GetQuickRepliesByUserIdQuery(userId))).FirstOrDefault();
        return quickReplyReadModel?.Shortcuts.Any(qr => qr.Shortcut == shortcut) == true;
    }

    private int GenerateShortcutId(List<QuickReplyShortcutItem>? shortcuts)
    {
        var existingIds = shortcuts?.Select(qr => qr.ShortcutId).ToList() ?? new List<int>();
        var newId = 1;
        while (existingIds.Contains(newId))
        {
            newId++;
        }
        return newId;
    }
}

