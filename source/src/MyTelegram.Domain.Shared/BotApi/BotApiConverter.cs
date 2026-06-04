using MyTelegram.Schema;
using MyTelegram.Schema.Messages;

namespace MyTelegram.Domain.Shared.BotApi;

/// <summary>
/// Converter between MTProto and Bot API formats
/// </summary>
public static class BotApiConverter
{
    /// <summary>
    /// Convert MTProto User to Bot API User
    /// </summary>
    public static BotApiUser ToBotApiUser(IUser user)
    {
        if (user is not Schema.TUser tUser)
        {
            throw new ArgumentException("Unsupported user type");
        }

        return new BotApiUser
        {
            Id = tUser.Id,
            IsBot = tUser.Bot,
            FirstName = tUser.FirstName ?? "",
            LastName = tUser.LastName,
            Username = tUser.Username,
            LanguageCode = null, // Not available in MTProto User
            IsPremium = tUser.Premium ? true : null,
            CanJoinGroups = tUser.Bot ? tUser.BotChatHistory : null,
            CanReadAllGroupMessages = tUser.Bot ? !tUser.BotNochats : null,
            SupportsInlineQueries = tUser.Bot ? tUser.BotInlinePlaceholder != null : null
        };
    }

    /// <summary>
    /// Convert MTProto Chat/Channel to Bot API Chat
    /// </summary>
    public static BotApiChat ToBotApiChat(IChat chat)
    {
        return chat switch
        {
            Schema.TChat tChat => new BotApiChat
            {
                Id = -tChat.Id, // Negative for groups
                Type = "group",
                Title = tChat.Title
            },
            Schema.TChannel tChannel => new BotApiChat
            {
                Id = tChannel.Broadcast ? -1000000000000 - tChannel.Id : -1000000000000 - tChannel.Id,
                Type = tChannel.Broadcast ? "channel" : "supergroup",
                Title = tChannel.Title,
                Username = tChannel.Username,
                IsForum = tChannel.Forum ? true : null
            },
            _ => throw new ArgumentException("Unsupported chat type")
        };
    }

    /// <summary>
    /// Convert MTProto Peer to Bot API Chat
    /// </summary>
    public static BotApiChat ToBotApiChat(long peerId, PeerType peerType, string? title = null, string? username = null)
    {
        return peerType switch
        {
            PeerType.User => new BotApiChat
            {
                Id = peerId,
                Type = "private",
                FirstName = title ?? "User"
            },
            PeerType.Chat => new BotApiChat
            {
                Id = -peerId,
                Type = "group",
                Title = title ?? "Group"
            },
            PeerType.Channel => new BotApiChat
            {
                Id = -1000000000000 - peerId,
                Type = "supergroup",
                Title = title ?? "Supergroup",
                Username = username
            },
            _ => throw new ArgumentException("Unsupported peer type")
        };
    }

    /// <summary>
    /// Convert MTProto Message to Bot API Message
    /// </summary>
    public static BotApiMessage ToBotApiMessage(Schema.IMessage message, Dictionary<long, IUser>? users = null, Dictionary<long, IChat>? chats = null)
    {
        if (message is not Schema.TMessage tMessage)
        {
            throw new ArgumentException("Unsupported message type");
        }

        // Extract peer info from TMessage.PeerId (which is IPeer)
        long peerId = 0;
        PeerType peerType = PeerType.User;
        
        if (tMessage.PeerId is Schema.TPeerUser peerUser)
        {
            peerId = peerUser.UserId;
            peerType = PeerType.User;
        }
        else if (tMessage.PeerId is Schema.TPeerChat peerChat)
        {
            peerId = peerChat.ChatId;
            peerType = PeerType.Chat;
        }
        else if (tMessage.PeerId is Schema.TPeerChannel peerChannel)
        {
            peerId = peerChannel.ChannelId;
            peerType = PeerType.Channel;
        }

        var botApiMessage = new BotApiMessage
        {
            MessageId = tMessage.Id,
            Date = tMessage.Date,
            Chat = ToBotApiChat(peerId, peerType),
            Text = tMessage.Message
        };

        // Add sender info
        if (tMessage.FromId != null && users != null)
        {
            long fromId = 0;
            if (tMessage.FromId is Schema.TPeerUser fromUser)
            {
                fromId = fromUser.UserId;
            }
            else if (tMessage.FromId is Schema.TPeerChat fromChat)
            {
                fromId = fromChat.ChatId;
            }
            else if (tMessage.FromId is Schema.TPeerChannel fromChannel)
            {
                fromId = fromChannel.ChannelId;
            }
            
            if (fromId > 0 && users.TryGetValue(fromId, out var user))
            {
                botApiMessage.From = ToBotApiUser(user);
            }
        }

        // Add entities
        if (tMessage.Entities != null && tMessage.Entities.Count > 0)
        {
            botApiMessage.Entities = tMessage.Entities.Select(ToBotApiMessageEntity).ToList();
        }

        // Add reply info
        if (tMessage.ReplyTo != null && tMessage.ReplyTo is Schema.TMessageReplyHeader replyHeader)
        {
            // Reply message would need to be fetched separately
            // For now, just indicate there's a reply
        }

        // Add edit date
        if (tMessage.EditDate.HasValue && tMessage.EditDate.Value > 0)
        {
            botApiMessage.EditDate = tMessage.EditDate.Value;
        }

        return botApiMessage;
    }

    /// <summary>
    /// Convert MTProto MessageEntity to Bot API MessageEntity
    /// </summary>
    public static BotApiMessageEntity ToBotApiMessageEntity(IMessageEntity entity)
    {
        var botApiEntity = new BotApiMessageEntity
        {
            Offset = entity.Offset,
            Length = entity.Length
        };

        botApiEntity.Type = entity switch
        {
            Schema.TMessageEntityBold => "bold",
            Schema.TMessageEntityItalic => "italic",
            Schema.TMessageEntityCode => "code",
            Schema.TMessageEntityPre => "pre",
            Schema.TMessageEntityTextUrl => "text_link",
            Schema.TMessageEntityMention => "mention",
            Schema.TMessageEntityHashtag => "hashtag",
            Schema.TMessageEntityBotCommand => "bot_command",
            Schema.TMessageEntityUrl => "url",
            Schema.TMessageEntityEmail => "email",
            Schema.TMessageEntityPhone => "phone_number",
            Schema.TMessageEntityCashtag => "cashtag",
            Schema.TMessageEntityUnderline => "underline",
            Schema.TMessageEntityStrike => "strikethrough",
            Schema.TMessageEntitySpoiler => "spoiler",
            Schema.TMessageEntityCustomEmoji => "custom_emoji",
            Schema.TMessageEntityBlockquote => "blockquote",
            _ => "unknown"
        };

        // Add entity-specific data
        if (entity is Schema.TMessageEntityTextUrl entityTextUrl)
        {
            botApiEntity.Url = entityTextUrl.Url;
        }
        else if (entity is Schema.TMessageEntityPre entityPre && !string.IsNullOrEmpty(entityPre.Language))
        {
            botApiEntity.Language = entityPre.Language;
        }
        else if (entity is Schema.TMessageEntityCustomEmoji entityCustomEmoji)
        {
            botApiEntity.CustomEmojiId = entityCustomEmoji.DocumentId.ToString();
        }

        return botApiEntity;
    }

    /// <summary>
    /// Convert Bot API chat ID to MTProto Peer
    /// </summary>
    public static (long PeerId, PeerType PeerType) FromBotApiChatId(long chatId)
    {
        if (chatId > 0)
        {
            // Private chat (user)
            return (chatId, PeerType.User);
        }
        else if (chatId > -1000000000000)
        {
            // Group chat
            return (-chatId, PeerType.Chat);
        }
        else
        {
            // Supergroup/Channel
            return (-chatId - 1000000000000, PeerType.Channel);
        }
    }

    /// <summary>
    /// Convert MTProto Updates to Bot API Update
    /// </summary>
    public static BotApiUpdate? ToBotApiUpdate(IUpdates updates, long updateId, Dictionary<long, IUser>? users = null, Dictionary<long, IChat>? chats = null)
    {
        if (updates is not TUpdates tUpdates)
        {
            return null;
        }

        var botApiUpdate = new BotApiUpdate
        {
            UpdateId = updateId
        };

        // Find message update
        var messageUpdate = tUpdates.Updates.OfType<Schema.TUpdateNewMessage>().FirstOrDefault();
        if (messageUpdate != null)
        {
            botApiUpdate.Message = ToBotApiMessage(messageUpdate.Message, users, chats);
            return botApiUpdate;
        }

        var editMessageUpdate = tUpdates.Updates.OfType<Schema.TUpdateEditMessage>().FirstOrDefault();
        if (editMessageUpdate != null)
        {
            botApiUpdate.EditedMessage = ToBotApiMessage(editMessageUpdate.Message, users, chats);
            return botApiUpdate;
        }

        var channelMessageUpdate = tUpdates.Updates.OfType<Schema.TUpdateNewChannelMessage>().FirstOrDefault();
        if (channelMessageUpdate != null)
        {
            botApiUpdate.ChannelPost = ToBotApiMessage(channelMessageUpdate.Message, users, chats);
            return botApiUpdate;
        }

        var editChannelMessageUpdate = tUpdates.Updates.OfType<Schema.TUpdateEditChannelMessage>().FirstOrDefault();
        if (editChannelMessageUpdate != null)
        {
            botApiUpdate.EditedChannelPost = ToBotApiMessage(editChannelMessageUpdate.Message, users, chats);
            return botApiUpdate;
        }

        return null;
    }

    /// <summary>
    /// Get update type from Bot API Update
    /// </summary>
    public static string GetUpdateType(BotApiUpdate update)
    {
        if (update.Message != null) return "message";
        if (update.EditedMessage != null) return "edited_message";
        if (update.ChannelPost != null) return "channel_post";
        if (update.EditedChannelPost != null) return "edited_channel_post";
        if (update.InlineQuery != null) return "inline_query";
        if (update.ChosenInlineResult != null) return "chosen_inline_result";
        if (update.CallbackQuery != null) return "callback_query";
        if (update.ShippingQuery != null) return "shipping_query";
        if (update.PreCheckoutQuery != null) return "pre_checkout_query";
        if (update.Poll != null) return "poll";
        if (update.PollAnswer != null) return "poll_answer";
        if (update.MyChatMember != null) return "my_chat_member";
        if (update.ChatMember != null) return "chat_member";
        if (update.ChatJoinRequest != null) return "chat_join_request";
        return "unknown";
    }
}
