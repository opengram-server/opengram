// ReSharper disable All

using MyTelegram.Domain.Aggregates.ChatTheme;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Change the chat theme of a certain chat
/// <para>Possible errors</para>
/// Code Type Description
/// 400 EMOJI_INVALID The specified theme emoji is valid.
/// 400 EMOJI_NOT_MODIFIED The theme wasn't changed.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.setChatTheme" />
///</summary>
internal sealed class SetChatThemeHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper,
    IIdGenerator idGenerator,
    IObjectMessageSender messageSender,
    IAccessHashHelper accessHashHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSetChatTheme, MyTelegram.Schema.IUpdates>,
        Messages.ISetChatThemeHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSetChatTheme obj)
    {
        // Resolve peer - must be a private chat (user)
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        if (peer == null)
        {
            RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
        }

        // Only private chats support themes
        if (peer.PeerType != PeerType.User)
        {
            RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
        }

        var otherUserId = peer.PeerId;

        // Validate emoticon
        if (string.IsNullOrWhiteSpace(obj.Emoticon))
        {
            RpcErrors.RpcErrors400.EmojiInvalid.ThrowRpcError();
        }

        // Check if emoticon is valid (belongs to available themes)
        var themesQuery = new GetAllChatThemesQuery();
        var availableThemes = await queryProcessor.ProcessAsync(themesQuery, CancellationToken.None);
        var validEmoticons = availableThemes.Select(t => t.Emoticon).ToHashSet();

        // Empty emoticon means clearing theme
        var isClearing = obj.Emoticon == string.Empty;

        if (!isClearing && !validEmoticons.Contains(obj.Emoticon))
        {
            RpcErrors.RpcErrors400.EmojiInvalid.ThrowRpcError();
        }

        // Check if theme is already set
        var currentThemeQuery = new GetChatThemeQuery(input.UserId, otherUserId);
        var currentTheme = await queryProcessor.ProcessAsync(currentThemeQuery, CancellationToken.None);

        if (!isClearing && currentTheme?.Emoticon == obj.Emoticon)
        {
            RpcErrors.RpcErrors400.EmojiNotModified.ThrowRpcError();
        }

        // Create message ID for service message
        var messageId = await idGenerator.NextIdAsync(IdType.MessageId, input.UserId, 1, CancellationToken.None);
        var date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Execute command
        // Generate deterministic GUID from user IDs to ensure same theme ID for same chat
        var combinedId = $"{Math.Min(input.UserId, otherUserId)}_{Math.Max(input.UserId, otherUserId)}";
        var guidBytes = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(combinedId));
        var themeGuid = new Guid(guidBytes);
        var chatThemeId = ChatThemeId.With($"chattheme-{themeGuid:D}");

        if (isClearing)
        {
            // Clear theme for BOTH users - create two separate commands
            
            // Command 1: Clear theme for current user
            var clearCommand1 = new ClearChatThemeCommand(
                chatThemeId,
                input.ToRequestInfo(),
                input.UserId,
                PeerType.User,
                otherUserId
            );
            await commandBus.PublishAsync(clearCommand1, CancellationToken.None);
            
            // Command 2: Clear theme for other user
            var guidBytes2 = new byte[16];
            Array.Copy(BitConverter.GetBytes(otherUserId), 0, guidBytes2, 0, 8);
            Array.Copy(BitConverter.GetBytes(input.UserId), 0, guidBytes2, 8, 8);
            var themeGuid2 = new Guid(guidBytes2);
            var chatThemeId2 = ChatThemeId.With($"chattheme-{themeGuid2:D}");
            
            var clearCommand2 = new ClearChatThemeCommand(
                chatThemeId2,
                input.ToRequestInfo(),
                otherUserId,
                PeerType.User,
                input.UserId
            );
            await commandBus.PublishAsync(clearCommand2, CancellationToken.None);
        }
        else
        {
            // Set theme for BOTH users in this chat - create two separate commands
            
            // Command 1: Set theme for current user (input.UserId)
            var setCommand1 = new SetChatThemeCommand(
                chatThemeId,          // aggregateId -> base
                input.ToRequestInfo(), // requestInfo -> base
                input.UserId,         // ownerPeerId (current user)
                PeerType.User,        // peerType
                otherUserId,          // peerId (other user)
                obj.Emoticon,         // emoticon
                messageId             // messageId
            );
            await commandBus.PublishAsync(setCommand1, CancellationToken.None);
            
            // Command 2: Set theme for other user (otherUserId)
            // Generate different chatThemeId for the other user
            var guidBytes2 = new byte[16];
            Array.Copy(BitConverter.GetBytes(otherUserId), 0, guidBytes2, 0, 8);
            Array.Copy(BitConverter.GetBytes(input.UserId), 0, guidBytes2, 8, 8);
            var themeGuid2 = new Guid(guidBytes2);
            var chatThemeId2 = ChatThemeId.With($"chattheme-{themeGuid2:D}");
            
            var setCommand2 = new SetChatThemeCommand(
                chatThemeId2,         // different aggregateId for other user
                input.ToRequestInfo(), // requestInfo -> base
                otherUserId,          // ownerPeerId (other user)
                PeerType.User,        // peerType
                input.UserId,         // peerId (current user)
                obj.Emoticon,         // emoticon
                messageId             // messageId
            );
            await commandBus.PublishAsync(setCommand2, CancellationToken.None);
        }

        // Create service message
        var messageAction = new TMessageActionSetChatTheme
        {
            Emoticon = obj.Emoticon
        };

        var serviceMessage = new MyTelegram.Schema.TMessageService
        {
            Id = messageId,
            FromId = new TPeerUser { UserId = input.UserId },
            PeerId = new TPeerUser { UserId = otherUserId },
            Date = date,
            Action = messageAction,
            Out = true
        };

        // Build updates response
        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = date,
            Seq = 0
        };

        // Add new message update
        var newMessageUpdate = new TUpdateNewMessage
        {
            Message = serviceMessage,
            Pts = 0, // Will be set by message system
            PtsCount = 1
        };

        updates.Updates.Add(newMessageUpdate);

        // Send notification to the other user
        var otherUserNotification = new TUpdateNewMessage
        {
            Message = new MyTelegram.Schema.TMessageService
            {
                Id = messageId,
                FromId = new TPeerUser { UserId = input.UserId },
                PeerId = new TPeerUser { UserId = input.UserId }, // From their perspective
                Date = date,
                Action = messageAction,
                Out = false
            },
            Pts = 0,
            PtsCount = 1
        };

        await messageSender.PushMessageToPeerAsync(
            new Peer(PeerType.User, otherUserId),
            otherUserNotification,
            excludeUserId: input.UserId
        );

        return updates;
    }
}
