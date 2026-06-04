using MyTelegram.Domain.Shared.Events;
using MyTelegram.Domain.Commands.Temp;
using MyTelegram.Domain.Aggregates.Temp;
using MyTelegram.Schema;
using MyTelegram.Messenger.CommandServer.Helpers;
using System.Text.Json;

namespace MyTelegram.Messenger.CommandServer.EventHandlers;

/// <summary>
/// Обрабатывает BotMessageEvent от Bot API и превращает его в StartSendMessageCommand.
/// </summary>
public class BotMessageEventHandler(
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    ILogger<BotMessageEventHandler> logger)
    : IEventHandler<BotMessageEvent>, ITransientDependency
{
    public async Task HandleEventAsync(BotMessageEvent eventData)
    {
        logger.LogInformation("Received BotMessageEvent from Bot API - BotId={BotId}, ChatId={ChatId}, Text={Text}",
            eventData.BotUserId, eventData.ChatId, eventData.Text);

        try
        {
            // Генерируем уникальный MessageId
            var messageId = await idGenerator.NextIdAsync(IdType.MessageId, eventData.BotUserId);
            var date = (int)eventData.Timestamp;

            // Собираем RequestInfo
            var requestInfo = new RequestInfo(
                ReqMsgId: eventData.RandomId,
                UserId: eventData.BotUserId,
                AccessHashKeyId: 0,
                AuthKeyId: 0,
                PermAuthKeyId: 0,
                RequestId: Guid.NewGuid(),
                Layer: 0,
                Date: eventData.Timestamp,
                DeviceType: DeviceType.Android,
                AddRequestIdToCache: false,
                IsSubRequest: false
            );

            // Готовим участников для MessageItem
            var ownerPeer = new Peer(PeerType.User, eventData.BotUserId);
            var toPeer = new Peer(eventData.PeerType, eventData.PeerId);
            var senderPeer = new Peer(PeerType.User, eventData.BotUserId);

            // Если ReplyMarkup передан в JSON, десериализуем его
            IReplyMarkup? replyMarkup = null;
            if (!string.IsNullOrEmpty(eventData.ReplyMarkupJson))
            {
                try
                {
                    replyMarkup = System.Text.Json.JsonSerializer.Deserialize<IReplyMarkup>(eventData.ReplyMarkupJson);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to deserialize ReplyMarkup JSON");
                }
            }

            var messageItem = new MessageItem(
                OwnerPeer: ownerPeer,
                ToPeer: toPeer,
                SenderPeer: senderPeer,
                SenderUserId: eventData.BotUserId,
                MessageId: messageId,
                Message: eventData.Text,
                Date: date,
                RandomId: eventData.RandomId,
                IsOut: true,
                SendMessageType: SendMessageType.Text,
                MessageType: MessageType.Text,
                MessageSubType: MessageSubType.Normal,
                InputReplyTo: eventData.ReplyToMessageId.HasValue
                    ? new TInputReplyToMessage
                    {
                        ReplyToMsgId = eventData.ReplyToMessageId.Value
                    }
                    : null,
                MessageAction: null,
                MessageActionType: MessageActionType.None,
                Entities: eventData.ParseMode == "Markdown" 
                    ? (MarkdownParser.ParseMarkdown(eventData.Text) is { } entities ? new TVector<IMessageEntity>(entities) : null)
                    : null,
                Media: null,
                GroupId: null,
                Post: false,
                FwdHeader: null,
                Views: null,
                PollId: null,
                ReplyMarkup: replyMarkup,
                LinkedChannelId: null,
                TopMsgId: null,
                PostAuthor: null,
                SavedPeerId: null,
                SendAs: null,
                Reply: null,
                EditHide: false,
                IsForwardFromChannelPost: false,
                PostChannelId: null,
                PostMessageId: null,
                QuickReplyItem: null,
                BatchId: null,
                Effect: null,
                Reactions: null,
                RecentReactions: null,
                EditDate: null,
                InboxItems: null,
                Pts: 0,
                ReplyToMsgItems: null,
                Silent: eventData.DisableNotification ?? false,
                ScheduleDate: null,
                ScheduleMessageId: null,
                TtlPeriod: null,
                IsTtlFromDefaultSetting: false,
                Pinned: false,
                InvertMedia: false,
                PublicPosts: false,
                Hashtags: null,
                MentionedUserIds: null
            );

            // Собираем SendMessageItem
            var sendMessageItem = new SendMessageItem(
                MessageItem: messageItem,
                ClearDraft: false,
                MentionedUserIds: null,
                ChatMembers: eventData.PeerType == PeerType.User ? new List<long> { eventData.PeerId } : null,
                SenderDefaultHistoryTTL: null
            );

            // Отправляем через CommandBus
            var command = new StartSendMessageCommand(
                TempId.New,
                requestInfo,
                new List<SendMessageItem> { sendMessageItem }
            );

            await commandBus.PublishAsync(command, CancellationToken.None);

            logger.LogInformation("Bot message command published successfully - MessageId={MessageId}, BotId={BotId}, ChatId={ChatId}",
                messageId, eventData.BotUserId, eventData.ChatId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle BotMessageEvent - BotId={BotId}, ChatId={ChatId}",
                eventData.BotUserId, eventData.ChatId);
            throw;
        }
    }
}
