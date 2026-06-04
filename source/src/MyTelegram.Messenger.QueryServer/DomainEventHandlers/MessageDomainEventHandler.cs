using MyTelegram.Messenger.Services.Caching;
using MyTelegram.Messenger.Services.Interfaces;

namespace MyTelegram.Messenger.QueryServer.DomainEventHandlers;

public partial class MessageDomainEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService,
    IChatEventCacheHelper chatEventCacheHelper,
    ILogger<MessageDomainEventHandler> logger,
    IQueryProcessor queryProcessor,
    IUpdatesConverterService updatesConverterService,
    IMessageConverterService messageConverterService,
    IUserConverterService userConverterService,
    IChatConverterService chatConverterService,
    IChannelAppService channelAppService,
    ISendMessageConverterService sendMessageConverterService,
    IEditMessageConverterService editMessageConverterService,
    IInviteToChannelConverterService inviteToChannelConverterService,
    IJoinChannelConverterService joinChannelConverterService,
    IPhotoAppService photoAppService)
    : DomainEventHandlerBase(objectMessageSender,
            commandBus,
            idGenerator,
            ackCacheService),
        ISubscribeSynchronousTo<EditMessageSaga, EditMessageSagaId, OutboxMessageEditCompletedSagaEvent>,
        ISubscribeSynchronousTo<EditMessageSaga, EditMessageSagaId, InboxMessageEditCompletedSagaEvent>,
        ISubscribeSynchronousTo<MessageAggregate, MessageId, ChannelMessagePinnedEvent>,
        ISubscribeSynchronousTo<MessageAggregate, MessageId, MessageReplyUpdatedEvent>,
        ISubscribeSynchronousTo<MessageAggregate, MessageId, MessageReactionAddedEvent>,
        ISubscribeSynchronousTo<MessageAggregate, MessageId, MessageReactionRemovedEvent>,
        ISubscribeSynchronousTo<SendMessageSaga, SendMessageSagaId, SendOutboxMessageCompletedSagaEvent>,
        ISubscribeSynchronousTo<SendMessageSaga, SendMessageSagaId, ReceiveInboxMessageCompletedSagaEvent>,
        ISubscribeSynchronousTo<MessageAggregate, MessageId, InboxMessageCreatedEvent>,
        ISubscribeSynchronousTo<MessageAggregate, MessageId, OutboxMessageCreatedEvent>
{
    public Task HandleAsync(
        IDomainEvent<EditMessageSaga, EditMessageSagaId, InboxMessageEditCompletedSagaEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var toPeer = domainEvent.AggregateEvent.NewMessageItem.OwnerPeer;
        var updates = editMessageConverterService.ToEditMessageUpdates(domainEvent.AggregateEvent);
        return PushUpdatesToPeerAsync(toPeer,
            updates,
            pts: domainEvent.AggregateEvent.NewMessageItem.Pts,
            updatesType: UpdatesType.Updates
        );
    }

    public async Task HandleAsync(
        IDomainEvent<EditMessageSaga, EditMessageSagaId, OutboxMessageEditCompletedSagaEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.NewMessageItem.QuickReplyItem != null)
        {
            await HandleEditQuickReplyMessageAsync(domainEvent.AggregateEvent);
            return;
        }

        var updates = editMessageConverterService.ToEditMessageUpdates(domainEvent.AggregateEvent.RequestInfo.UserId, domainEvent.AggregateEvent, domainEvent.AggregateEvent.RequestInfo.Layer);
        var updatesForOtherParticipant = editMessageConverterService.ToEditMessageUpdates(-1, domainEvent.AggregateEvent, 0);

        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo,
            updates,
            domainEvent.AggregateEvent.NewMessageItem.SenderPeer.PeerId,
            domainEvent.AggregateEvent.NewMessageItem.Pts,
            domainEvent.AggregateEvent.NewMessageItem.ToPeer.PeerType
        );
        await PushUpdatesToPeerAsync(
            domainEvent.AggregateEvent.NewMessageItem.SenderUserId.ToUserPeer(),
            updatesForOtherParticipant,
            domainEvent.AggregateEvent.RequestInfo.AuthKeyId,
            pts: domainEvent.AggregateEvent.NewMessageItem.Pts,
            updatesType: UpdatesType.Updates);

        // Сообщение канала использует тот же объект сообщения: при редактировании исходящего нужно уведомить участников канала
        if (domainEvent.AggregateEvent.NewMessageItem.ToPeer.PeerType == PeerType.Channel)
        {
            var channelEditUpdates = editMessageConverterService.ToEditMessageUpdates(-1, domainEvent.AggregateEvent, 0);

            await PushUpdatesToPeerAsync(domainEvent.AggregateEvent.NewMessageItem.ToPeer,
                channelEditUpdates,
                pts: domainEvent.AggregateEvent.NewMessageItem.Pts);
        }
    }

    public Task HandleAsync(IDomainEvent<MessageAggregate, MessageId, ChannelMessagePinnedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task HandleAsync(IDomainEvent<MessageAggregate, MessageId, MessageReplyUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var messageReadModel = await queryProcessor.ProcessAsync(new GetMessageByIdQuery(MessageId
                .Create(domainEvent.AggregateEvent.OwnerChannelId, domainEvent.AggregateEvent.MessageId).Value),
            cancellationToken);

        if (messageReadModel != null)
        {
            //var message = messageLayeredService.Converter.ToMessage(messageReadModel, null, null, 0);

            var message = messageConverterService.ToMessage(messageReadModel.SenderUserId, messageReadModel);
            if (message is TMessage tMessage)
            {
                tMessage.EditDate = DateTime.UtcNow.ToTimestamp();
                tMessage.EditHide = true;
            }

            var update = new TUpdateEditChannelMessage
            {
                Message = message,
                Pts = domainEvent.AggregateEvent.Pts,
                PtsCount = 1
            };
            var updates = new TUpdates
            {
                Updates = new TVector<IUpdate>(update),
                Users = [],
                Chats = [],
                Date = DateTime.UtcNow.ToTimestamp()
            };

            await PushUpdatesToPeerAsync(domainEvent.AggregateEvent.OwnerChannelId.ToChannelPeer(), updates);
        }
    }

    public async Task HandleAsync(IDomainEvent<MessageAggregate, MessageId, MessageReactionAddedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;

        logger.LogInformation("*** MessageReactionAddedEvent received: OwnerPeerId={OwnerPeerId}, MessageId={MessageId}, SenderUserId={SenderUserId}",
            evt.OwnerPeerId, evt.MessageId, evt.SenderUserId);

        // Получаем из ReadModel обновлённое сообщение вместе с реакциями
        var messageReadModel = await queryProcessor.ProcessAsync(
            new GetMessageByIdQuery(MessageId.Create(evt.OwnerPeerId, evt.MessageId, false).Value),
            cancellationToken);

        if (messageReadModel == null)
        {
            logger.LogWarning("Message not found for reaction update: OwnerPeerId={OwnerPeerId}, MessageId={MessageId}",
                evt.OwnerPeerId, evt.MessageId);
            return;
        }

        // Формируем реакции сообщения по данным из ReadModel
        var messageReactions = CreateMessageReactionsFromReadModel(messageReadModel);

        // Формируем update
        var update = new TUpdateMessageReactions
        {
            Peer = new Peer(messageReadModel.ToPeerType, messageReadModel.ToPeerId).ToPeer(),
            MsgId = evt.MessageId,
            Reactions = messageReactions
        };

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = evt.Date,
            Seq = 0
        };

        logger.LogInformation("*** Sending TUpdateMessageReactions: ToPeerType={ToPeerType}, ToPeerId={ToPeerId}, MessageId={MessageId}, ReactionsCount={Count}",
            messageReadModel.ToPeerType, messageReadModel.ToPeerId, evt.MessageId, messageReadModel.Reactions?.Count ?? 0);

        // Для личных чатов отправляем update получателю (ToPeer),
        // для каналов - всем участникам канала
        var toPeer = new Peer(messageReadModel.ToPeerType, messageReadModel.ToPeerId);

        if (messageReadModel.ToPeerType == PeerType.Channel)
        {
            // Для каналов отправляем всем участникам
            await PushUpdatesToPeerAsync(
                toPeer,
                updates,
                excludeAuthKeyId: evt.RequestInfo.AuthKeyId,
                updatesType: UpdatesType.Updates
            );
        }
        else
        {
            // Для личных и групповых чатов отправляем получателю (ToPeer)
            await PushUpdatesToPeerAsync(
                toPeer,
                updates,
                excludeAuthKeyId: evt.RequestInfo.AuthKeyId, // Исключаем текущее устройство отправителя
                updatesType: UpdatesType.Updates
            );

            // Всегда отправляем на ОСТАЛЬНЫЕ устройства отправителя (кроме текущего)
            var fromPeer = new Peer(PeerType.User, evt.SenderUserId);
            logger.LogInformation("*** Sending reaction update to sender's other devices: SenderUserId={SenderUserId}", evt.SenderUserId);
            await PushUpdatesToPeerAsync(
                fromPeer,
                updates,
                excludeAuthKeyId: evt.RequestInfo.AuthKeyId, // Исключаем текущее устройство, отправляем на остальные
                updatesType: UpdatesType.Updates
            );
        }
    }

    public async Task HandleAsync(IDomainEvent<MessageAggregate, MessageId, MessageReactionRemovedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;

        logger.LogInformation("*** MessageReactionRemovedEvent received: OwnerPeerId={OwnerPeerId}, MessageId={MessageId}, SenderUserId={SenderUserId}",
            evt.OwnerPeerId, evt.MessageId, evt.SenderUserId);

        // Получаем из ReadModel обновлённое сообщение вместе с реакциями
        var messageReadModel = await queryProcessor.ProcessAsync(
            new GetMessageByIdQuery(MessageId.Create(evt.OwnerPeerId, evt.MessageId, false).Value),
            cancellationToken);

        if (messageReadModel == null)
        {
            logger.LogWarning("Message not found for reaction removal update: OwnerPeerId={OwnerPeerId}, MessageId={MessageId}",
                evt.OwnerPeerId, evt.MessageId);
            return;
        }

        // Формируем реакции сообщения по данным из ReadModel
        var messageReactions = CreateMessageReactionsFromReadModel(messageReadModel);

        // Формируем update
        var update = new TUpdateMessageReactions
        {
            Peer = new Peer(messageReadModel.ToPeerType, messageReadModel.ToPeerId).ToPeer(),
            MsgId = evt.MessageId,
            Reactions = messageReactions
        };

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Seq = 0
        };

        logger.LogInformation("*** Sending TUpdateMessageReactions (removed): ToPeerType={ToPeerType}, ToPeerId={ToPeerId}, MessageId={MessageId}, ReactionsCount={Count}",
            messageReadModel.ToPeerType, messageReadModel.ToPeerId, evt.MessageId, messageReadModel.Reactions?.Count ?? 0);

        var toPeer = new Peer(messageReadModel.ToPeerType, messageReadModel.ToPeerId);

        if (messageReadModel.ToPeerType == PeerType.Channel)
        {
            // Для каналов отправляем всем участникам
            await PushUpdatesToPeerAsync(
                toPeer,
                updates,
                excludeAuthKeyId: evt.RequestInfo.AuthKeyId,
                updatesType: UpdatesType.Updates
            );
        }
        else
        {
            // Для личных и групповых чатов отправляем получателю (ToPeer)
            await PushUpdatesToPeerAsync(
                toPeer,
                updates,
                excludeAuthKeyId: evt.RequestInfo.AuthKeyId, // Исключаем текущее устройство отправителя
                updatesType: UpdatesType.Updates
            );

            // Всегда отправляем на ОСТАЛЬНЫЕ устройства отправителя (кроме текущего)
            var fromPeer = new Peer(PeerType.User, evt.SenderUserId);
            logger.LogInformation("*** Sending reaction removal update to sender's other devices: SenderUserId={SenderUserId}", evt.SenderUserId);
            await PushUpdatesToPeerAsync(
                fromPeer,
                updates,
                excludeAuthKeyId: evt.RequestInfo.AuthKeyId, // Исключаем текущее устройство, отправляем на остальные
                updatesType: UpdatesType.Updates
            );
        }
    }

    private IMessageReactions CreateMessageReactionsFromReadModel(IMessageReadModel message)
    {
        if (message.Reactions == null || message.Reactions.Count == 0)
        {
            return new TMessageReactions
            {
                Results = new TVector<IReactionCount>(),
                CanSeeList = false
            };
        }

        var results = message.Reactions.Select(r => new TReactionCount
        {
            Reaction = r.Reaction,
            Count = r.Count
        }).ToList();

        // Для каналов не показываем, кто поставил реакцию (анонимные реакции),
        // для групп и личных чатов показываем последние реакции с пользователями
        var isChannel = message.ToPeerType == PeerType.Channel;
        
        var recentReactions = !isChannel && message.RecentReactions2 != null
            ? message.RecentReactions2.Select(r => new TMessagePeerReaction
            {
                Big = r.Big,
                PeerId = r.PeerId.ToPeer(),
                Date = r.Date,
                Reaction = r.Reaction
            }).ToList()
            : null;

        return new TMessageReactions
        {
            Results = new TVector<IReactionCount>(results),
            RecentReactions = recentReactions != null ? new TVector<IMessagePeerReaction>(recentReactions) : null,
            CanSeeList = !isChannel && message.Reactions?.Any() == true
        };
    }

    private Task<ILayeredUser> GetUserAsync(IRequestWithAccessHashKeyId request, long userId/*, long selfUserId*/)
    {
        return userConverterService.GetUserAsync(request, userId);
    }

    private async Task HandleCreateChannelAsync(SendOutboxMessageCompletedSagaEvent aggregateEvent)
    {
        var item = aggregateEvent.MessageItem;
        if (chatEventCacheHelper.TryRemoveChannelCreatedEvent(item.ToPeer.PeerId,
                out var eventData))
        {
            var channelId = eventData.ChannelId;
            //var updates = updatesLayeredService.GetConverter(aggregateEvent.RequestInfo.Layer)
            //    .ToCreateChannelUpdates(eventData, aggregateEvent);
            var updates = await updatesConverterService.ToCreateChannelUpdatesAsync(eventData, aggregateEvent, true, aggregateEvent.RequestInfo.Layer);
            var updatesForSelfOtherDevices = await updatesConverterService.ToCreateChannelUpdatesAsync(eventData, aggregateEvent, true, 0);
            IObject rpcData = updates;

            if (item.MessageSubType == MessageSubType.AutoCreateChannelFromChat)
            {
                var invitedUsers = new TInvitedUsers
                {
                    Updates = updates,
                    MissingInvitees = []
                };

                rpcData = invitedUsers;
            }

            await SendRpcMessageToClientAsync(aggregateEvent.RequestInfo,
                rpcData,
                item.SenderPeer.PeerId
            );

            await PushUpdatesToChannelSingleMemberAsync(channelId, item.SenderPeer,
                updatesForSelfOtherDevices,
                aggregateEvent.RequestInfo.AuthKeyId,
                pts: item.Pts
            );
        }
        else
        {
            logger.LogWarning("Cannot find cached channel info, channelId: {ChannelId}",
                item.ToPeer.PeerId);
        }
    }

    private async Task HandleEditQuickReplyMessageAsync(OutboxMessageEditCompletedSagaEvent aggregateEvent)
    {
        var updates = editMessageConverterService.ToEditQuickReplyMessageUpdates(aggregateEvent, aggregateEvent.RequestInfo.Layer);
        var updatesForSelfOtherDevices = editMessageConverterService.ToEditQuickReplyMessageUpdates(aggregateEvent, 0);
        await SendRpcMessageToClientAsync(aggregateEvent.RequestInfo, updates);
        await PushUpdatesToPeerAsync(aggregateEvent.RequestInfo.UserId.ToUserPeer(), updatesForSelfOtherDevices,
            aggregateEvent.RequestInfo.PermAuthKeyId);
    }
    private async Task HandleForwardMessageAsync(ReceiveInboxMessageCompletedSagaEvent aggregateEvent)
    {
        var item = aggregateEvent.MessageItem;

        var updates = updatesConverterService.ToInboxForwardMessageUpdates(aggregateEvent);
        if (aggregateEvent.MessageItem.FwdHeader?.FromId?.PeerType == PeerType.Channel)
        {
            if (updates is TUpdates tUpdates)
            {
                var channelId = aggregateEvent.MessageItem.FwdHeader.FromId.PeerId;
                var channel = await chatConverterService.GetChannelAsync(RequestInfo.Empty, channelId, true, null);

                tUpdates.Chats.Add(channel);
            }
        }
        await PushUpdatesToPeerAsync(item.OwnerPeer,
            updates,
            pts: item.Pts);
    }

    private async Task HandleInviteToChannelAsync(SendOutboxMessageCompletedSagaEvent aggregateEvent)
    {
        var item = aggregateEvent.MessageItem;
        var invitedUsers = inviteToChannelConverterService.ToInvitedUsers(aggregateEvent);
        var invitedUserIds = new List<long>();
        if (item.MessageAction is TMessageActionChatAddUser messageActionChatAddUser)
        {
            invitedUserIds = messageActionChatAddUser.Users.ToList();
            invitedUserIds.Add(item.SenderUserId);
        }

        await UpdateChannelAndUserAsync(aggregateEvent.RequestInfo, invitedUsers.Updates, item.ToPeer.PeerId, invitedUserIds, layer: aggregateEvent.RequestInfo.Layer);
        await SendRpcMessageToClientAsync(aggregateEvent.RequestInfo,
            invitedUsers,
            item.SenderPeer.PeerId);
        var updatesForSelfOtherDevices = inviteToChannelConverterService.ToInviteToChannelUpdates(aggregateEvent, 0);
        await UpdateChannelAndUserAsync(aggregateEvent.RequestInfo, updatesForSelfOtherDevices, item.ToPeer.PeerId, invitedUserIds, layer: aggregateEvent.RequestInfo.Layer);

        await PushMessageToPeerAsync(item.ToPeer, updatesForSelfOtherDevices, excludeAuthKeyId: aggregateEvent.RequestInfo.PermAuthKeyId);


        var updatesForChannelMember = inviteToChannelConverterService.ToInviteToChannelUpdates(aggregateEvent, 0);
        await UpdateChannelAndUserAsync(RequestInfo.Empty with { UserId = -1 }, updatesForChannelMember, item.ToPeer.PeerId, invitedUserIds);

        foreach (var userId in invitedUserIds)
        {
            if (userId == aggregateEvent.RequestInfo.UserId)
            {
                continue;
            }

            await PushUpdatesToChannelSingleMemberAsync(item.ToPeer.PeerId, userId.ToUserPeer(),
                updatesForChannelMember);
        }
        if (item.Post)
        {
            return;
        }
        await PushMessageToPeerAsync(item.ToPeer, updatesForChannelMember, excludeUserId: aggregateEvent.RequestInfo.UserId);
    }

    private async Task HandleReceiveMessageAsync(ReceiveInboxMessageCompletedSagaEvent aggregateEvent)
    {
        var updates = sendMessageConverterService.ToUpdates(aggregateEvent);
        var item = aggregateEvent.MessageItems.Last();
        await PushUpdatesToPeerAsync(item.OwnerPeer,
            updates,
            pts: item.Pts,
            senderUserId: item.SenderPeer.PeerId
        );
    }

    private Task HandleReceiveMessageCompletedAsync(ReceiveInboxMessageCompletedSagaEvent aggregateEvent)
    {
        var item = aggregateEvent.MessageItem;
        return item.MessageSubType switch
        {
            MessageSubType.UpdatePinnedMessage => HandleUpdatePinnedMessageAsync(aggregateEvent),
            MessageSubType.ForwardMessage => HandleForwardMessageAsync(aggregateEvent),
            _ => HandleReceiveMessageAsync(aggregateEvent)
        };
    }

    private async Task HandleSendMessageAsync(SendOutboxMessageCompletedSagaEvent aggregateEvent)
    {
        var item = aggregateEvent.MessageItem;
        if (item.ToPeer.PeerType == PeerType.Channel)
        {
            await HandleSendMessageToChannelAsync(aggregateEvent);
            return;
        }

        var layer = aggregateEvent.RequestInfo.Layer;
        if (aggregateEvent.RequestInfo.ReqMsgId == 0 || item.MessageSubType == MessageSubType.PhoneCall)
        {
            layer = 0;
        }

        var selfUpdates = sendMessageConverterService.ToUpdates(aggregateEvent, layer);
        var selfOtherDeviceUpdates = sendMessageConverterService.ToSelfOtherDeviceUpdates(aggregateEvent, 0);

        var updatesType = UpdatesType.Updates;
        if (item.MessageSubType == MessageSubType.Normal ||
            item.MessageSubType == MessageSubType.ForwardMessage)
        {
            updatesType = UpdatesType.NewMessages;
        }

        // Когда reqMsgId == 0?
        // Пересланное сообщение или сообщение от бота
        if (aggregateEvent.RequestInfo.ReqMsgId == 0 || item.MessageSubType == MessageSubType.PhoneCall)
        {
            await PushUpdatesToPeerAsync(item.SenderPeer,
                selfUpdates,
                pts: item.Pts,
                updatesType: updatesType
            );
        }
        else
        {
            await ReplyRpcResultToSenderAsync(aggregateEvent.RequestInfo,
                item.SenderPeer,
                selfUpdates,
                item.SenderPeer.PeerId,
                item.Pts
            );
        }


        long? excludeAuthKeyId = aggregateEvent.RequestInfo.AuthKeyId;
        if (item.MessageActionType == MessageActionType.SetChatTheme)
        {
            excludeAuthKeyId = null;
        }

        await PushUpdatesToPeerAsync(item.SenderPeer,
            selfOtherDeviceUpdates,
            excludeAuthKeyId,
            pts: item.Pts,
            updatesType: updatesType
        );
    }

    private void SetChannelInfo(IRequestWithAccessHashKeyId request,
        IUpdates updates,
        IChannelReadModel? channelReadModel,
        IPhotoReadModel? photoReadModel,
        int layer)
    {
        if (channelReadModel == null)
        {
            return;
        }

        if (updates is TUpdates tUpdates)
        {
            if (tUpdates.Chats.All(p => p.Id != channelReadModel.ChannelId))
            {
                var channel =
                    chatConverterService.ToChannel(request, channelReadModel, photoReadModel, null, false, layer);
                tUpdates.Chats.Add(channel);
            }
        }
    }

    private async Task HandleSendMessageToChannelAsync(SendOutboxMessageCompletedSagaEvent aggregateEvent)
    {
        var item = aggregateEvent.MessageItem;
        var selfUpdates = sendMessageConverterService.ToUpdates(aggregateEvent, aggregateEvent.RequestInfo.Layer);
        var selfOtherDeviceUpdates = sendMessageConverterService.ToSelfOtherDeviceUpdates(aggregateEvent, 0);
        //var channelUpdates = updatesConverterService.ToChannelMessageUpdates(aggregateEvent.RequestInfo.UserId, aggregateEvent, aggregateEvent.RequestInfo.Layer);
        var channelAdminUpdates = updatesConverterService.ToChannelMessageUpdates(-1, aggregateEvent, 0);
        var channelMemberUpdates = updatesConverterService.ToChannelMessageUpdates(-1, aggregateEvent, 0);
        var channelReadModel = await channelAppService.GetAsync(item.ToPeer.PeerId);
        IChannelReadModel? sendAsReadModel = null;
        IChannelReadModel? forwardFromChannelReadModel = null;

        IPhotoReadModel? sendAsPhotoReadModel = null;
        IPhotoReadModel? forwardFromChannelPhotoReadModel = null;

        if (item.SendAs?.PeerType == PeerType.Channel)
        {
            sendAsReadModel = await channelAppService.GetAsync(item.SendAs.PeerId);
            sendAsPhotoReadModel = await photoAppService.GetAsync(sendAsReadModel?.PhotoId);
        }

        if (item.FwdHeader?.FromId?.PeerType == PeerType.Channel)
        {
            forwardFromChannelReadModel = await channelAppService.GetAsync(item.FwdHeader.FromId.PeerId);
            forwardFromChannelPhotoReadModel = await photoAppService.GetAsync((long?)item.FwdHeader.FromId.PeerId);
        }

        var photoReadModel = await photoAppService.GetAsync(channelReadModel.PhotoId);
        var layer = aggregateEvent.RequestInfo.Layer;

        SetChannelInfo(aggregateEvent.RequestInfo, selfUpdates, channelReadModel, photoReadModel, layer);
        SetChannelInfo(aggregateEvent.RequestInfo, selfUpdates, sendAsReadModel, sendAsPhotoReadModel, layer);
        SetChannelInfo(aggregateEvent.RequestInfo, selfUpdates, forwardFromChannelReadModel, forwardFromChannelPhotoReadModel, layer);

        SetChannelInfo(aggregateEvent.RequestInfo, selfOtherDeviceUpdates, channelReadModel, photoReadModel, 0);
        SetChannelInfo(aggregateEvent.RequestInfo, selfOtherDeviceUpdates, sendAsReadModel, sendAsPhotoReadModel, 0);
        SetChannelInfo(aggregateEvent.RequestInfo, selfOtherDeviceUpdates, forwardFromChannelReadModel, forwardFromChannelPhotoReadModel, 0);

        SetChannelInfo(RequestInfo.Empty with { UserId = -1 }, channelMemberUpdates, channelReadModel, photoReadModel, 0);
        SetChannelInfo(RequestInfo.Empty with { UserId = -1 }, channelMemberUpdates, sendAsReadModel, sendAsPhotoReadModel, 0);
        SetChannelInfo(RequestInfo.Empty with { UserId = -1 }, channelMemberUpdates, forwardFromChannelReadModel, forwardFromChannelPhotoReadModel, 0);

        var updatesType = UpdatesType.Updates;
        if (item.MessageSubType == MessageSubType.Normal || item.MessageSubType == MessageSubType.ForwardMessage)
        {
            updatesType = UpdatesType.NewMessages;
        }

        var globalSeqNo = await SavePushUpdatesAsync(item.ToPeer.PeerId,
            channelMemberUpdates,
            item.Pts,
            aggregateEvent.RequestInfo.AuthKeyId,
            aggregateEvent.RequestInfo.UserId,
            messageId: item.MessageId
        );
        await AddRpcGlobalSeqNoForAuthKeyIdAsync(aggregateEvent.RequestInfo.ReqMsgId, item.SenderPeer.PeerId,
                globalSeqNo)
            ;

        if (aggregateEvent.RequestInfo.ReqMsgId == 0 /*|| item.MessageSubType == MessageSubType.ForwardMessage*/)
        {
            await PushUpdatesToPeerAsync(new Peer(PeerType.User, aggregateEvent.RequestInfo.UserId),
                selfOtherDeviceUpdates,
                pts: item.Pts,
                updatesType: updatesType
            );
        }
        else
        {
            await ReplyRpcResultToSenderAsync(aggregateEvent.RequestInfo,
                item.SenderUserId.ToUserPeer(),
                selfUpdates,
                aggregateEvent.RequestInfo.UserId,
                item.Pts
            );

            await PushUpdatesToPeerAsync(item.SenderUserId.ToUserPeer(),
                selfOtherDeviceUpdates,
                aggregateEvent.RequestInfo.PermAuthKeyId,
                pts: item.Pts,
                updatesType: updatesType
            );
        }

        // Уведомляем упомянутых пользователей
        if (aggregateEvent.MentionedUserIds?.Count > 0)
        {
            foreach (var mentionedUserId in aggregateEvent.MentionedUserIds)
            {
                var mentionedUpdates = updatesConverterService.ToChannelMessageUpdates(mentionedUserId, aggregateEvent, 0, true);
                SetChannelInfo(RequestInfo.Empty with { UserId = mentionedUserId }, mentionedUpdates, channelReadModel, photoReadModel, 0);
                SetChannelInfo(RequestInfo.Empty with { UserId = mentionedUserId }, mentionedUpdates, sendAsReadModel, sendAsPhotoReadModel, 0);
                SetChannelInfo(RequestInfo.Empty with { UserId = mentionedUserId }, mentionedUpdates, forwardFromChannelReadModel, forwardFromChannelPhotoReadModel, 0);
                await PushUpdatesToPeerAsync(mentionedUserId.ToUserPeer(), mentionedUpdates);
            }
        }

        var adminUserIds = channelReadModel.AdminList.Select(p => p.UserId).ToList();
        adminUserIds.Insert(0, channelReadModel.CreatorId);

        foreach (var adminUserId in adminUserIds)
        {
            if (adminUserId == aggregateEvent.RequestInfo.UserId)
            {
                continue;
            }

            if (aggregateEvent.MentionedUserIds?.Contains(adminUserId) ?? false)
            {
                continue;
            }

            SetChannelInfo(RequestInfo.Empty with { UserId = adminUserId }, channelAdminUpdates, channelReadModel, photoReadModel, 0);
            await PushUpdatesToPeerAsync(item.ToPeer,
                channelAdminUpdates,
                aggregateEvent.RequestInfo.AuthKeyId,
                updatesType: updatesType,
                skipSaveUpdates: true
            );
        }

        // Сообщение о выходе участника отправляем только администраторам группы
        if (aggregateEvent.MessageItem.MessageActionType == MessageActionType.ChatDeleteUser)
        {
            return;
        }

        await PushUpdatesToPeerAsync(item.ToPeer,
                channelMemberUpdates,
                excludeUserId:aggregateEvent.RequestInfo.UserId,
                updatesType: updatesType,
                skipSaveUpdates: true,
                excludeUserIds: aggregateEvent.MentionedUserIds
            )
            ;
    }

    private Task HandleSendOutboxMessageCompletedAsync(SendOutboxMessageCompletedSagaEvent aggregateEvent)
    {
        var item = aggregateEvent.MessageItem;
        return item.MessageSubType switch
        {
            //MessageSubType.CreateChat => HandleCreateChatAsync(aggregateEvent),
            MessageSubType.CreateChannel => HandleCreateChannelAsync(aggregateEvent),
            MessageSubType.AutoCreateChannelFromChat => HandleCreateChannelAsync(aggregateEvent),
            MessageSubType.InviteToChannel => HandleInviteToChannelAsync(aggregateEvent),
            MessageSubType.UpdatePinnedMessage => HandleUpdatePinnedMessageAsync(aggregateEvent),
            MessageSubType.ChatJoinByLink => HandleJoinChannelAsync(aggregateEvent),
            MessageSubType.ChatJoinBySelf => HandleJoinChannelAsync(aggregateEvent),
            _ => HandleSendMessageAsync(aggregateEvent)
        };
    }

    private async Task HandleUpdatePinnedMessageAsync(ReceiveInboxMessageCompletedSagaEvent aggregateEvent)
    {
        var updates = updatesConverterService.ToUpdatePinnedMessageUpdates(aggregateEvent);
        var item = aggregateEvent.MessageItem;
        await PushUpdatesToPeerAsync(item.OwnerPeer,
            updates,
            pts: item.Pts);
    }

    private async Task HandleUpdatePinnedMessageAsync(SendOutboxMessageCompletedSagaEvent aggregateEvent)
    {
        var item = aggregateEvent.MessageItem;
        var updates = updatesConverterService.ToUpdatePinnedMessageUpdates(aggregateEvent, 0);

        await PushUpdatesToPeerAsync(item.SenderPeer,
            updates,
            pts: item.Pts);

        if (item.ToPeer.PeerType == PeerType.Channel)
        {
            var channelUpdates = updatesConverterService.ToUpdatePinnedMessageServiceUpdates(0, aggregateEvent, 0);
            if (channelUpdates is TUpdates tUpdates)
            {
                var user = await GetUserAsync(aggregateEvent.RequestInfo, aggregateEvent.RequestInfo.UserId);
                tUpdates.Users.Add(user);
            }

            await PushUpdatesToPeerAsync(item.ToPeer,
                channelUpdates,
                aggregateEvent.RequestInfo.AuthKeyId,
                pts: item.Pts
            );
        }
    }

    public Task HandleAsync(
        IDomainEvent<SendMessageSaga, SendMessageSagaId, ReceiveInboxMessageCompletedSagaEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        return HandleReceiveMessageCompletedAsync(domainEvent.AggregateEvent);
    }

    public Task HandleAsync(
        IDomainEvent<SendMessageSaga, SendMessageSagaId, SendOutboxMessageCompletedSagaEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        return HandleSendOutboxMessageCompletedAsync(domainEvent.AggregateEvent);
    }

    private async Task UpdateChannelAndUserAsync(RequestInfo requestInfo, IUpdates updates, long channelId, List<long>? userIds, int layer = 0)
    {
        if (updates is TUpdates tUpdates)
        {
            if (tUpdates.Chats.Count == 0)
            {
                var channel = await chatConverterService.GetChannelAsync(requestInfo, channelId, false, false, layer);
                tUpdates.Chats = [channel];
            }

            if (userIds?.Count > 0)
            {
                var users = await userConverterService.GetUserListAsync(requestInfo, userIds, layer: layer);
                foreach (var layeredUser in users)
                {
                    tUpdates.Users.Add(layeredUser);
                }
            }
        }
    }

    public async Task HandleAsync(
        IDomainEvent<MessageAggregate, MessageId, InboxMessageCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        var item = evt.InboxMessageItem;

        Console.WriteLine($"[MessageDomainEventHandler] InboxMessageCreatedEvent received! OwnerPeerId={item.OwnerPeer.PeerId}, MessageId={item.MessageId}, MessageActionType={item.MessageActionType}, Pts={item.Pts}");
        logger.LogInformation("InboxMessageCreatedEvent received: OwnerPeerId={OwnerPeerId}, MessageId={MessageId}, MessageActionType={MessageActionType}, Pts={Pts}",
            item.OwnerPeer.PeerId, item.MessageId, item.MessageActionType, item.Pts);

        // Готовим апдейты для входящего сообщения (получатель)
        var updates = await CreateInboxMessageUpdatesAsync(item);

        Console.WriteLine($"[MessageDomainEventHandler] Sending updates to recipient: OwnerPeerId={item.OwnerPeer.PeerId}, Pts={item.Pts}");

        // Отправляем update получателю
        await PushUpdatesToPeerAsync(
            item.OwnerPeer,
            updates,
            pts: item.Pts,
            senderUserId: item.SenderPeer.PeerId,
            updatesType: UpdatesType.Updates
        );

        Console.WriteLine($"[MessageDomainEventHandler] Updates sent to recipient: OwnerPeerId={item.OwnerPeer.PeerId}");
        logger.LogInformation("InboxMessageCreatedEvent: Updates sent to recipient OwnerPeerId={OwnerPeerId}", item.OwnerPeer.PeerId);
    }

    public async Task HandleAsync(
        IDomainEvent<MessageAggregate, MessageId, OutboxMessageCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        var item = evt.OutboxMessageItem;

        Console.WriteLine($"[MessageDomainEventHandler] OutboxMessageCreatedEvent received! OwnerPeerId={item.OwnerPeer.PeerId}, MessageId={item.MessageId}, MessageActionType={item.MessageActionType}, Pts={item.Pts}");
        logger.LogInformation("OutboxMessageCreatedEvent received: OwnerPeerId={OwnerPeerId}, MessageId={MessageId}, MessageActionType={MessageActionType}, Pts={Pts}",
            item.OwnerPeer.PeerId, item.MessageId, item.MessageActionType, item.Pts);

        // Готовим апдейты для исходящего сообщения (отправитель)
        var updates = await CreateOutboxMessageUpdatesAsync(item);

        Console.WriteLine($"[MessageDomainEventHandler] Sending updates to sender: OwnerPeerId={item.OwnerPeer.PeerId}, Pts={item.Pts}");

        // Отправляем update отправителю
        await PushUpdatesToPeerAsync(
            item.OwnerPeer,
            updates,
            pts: item.Pts,
            updatesType: UpdatesType.Updates
        );

        Console.WriteLine($"[MessageDomainEventHandler] Updates sent to sender: OwnerPeerId={item.OwnerPeer.PeerId}");
        logger.LogInformation("OutboxMessageCreatedEvent: Updates sent to sender OwnerPeerId={OwnerPeerId}", item.OwnerPeer.PeerId);
    }

    private async Task<IUpdates> CreateInboxMessageUpdatesAsync(MessageItem item)
    {
        // Для входящих сообщений формируем TUpdateNewMessage
        var message = messageConverterService.ToMessage(0, item);
        var update = new TUpdateNewMessage
        {
            Message = message,
            Pts = item.Pts,
            PtsCount = 1
        };

        // Подгружаем данные отправителя в апдейты (иначе пропадают Premium и статус)
        var users = new TVector<IUser>();
        if (item.SenderPeer.PeerType == PeerType.User)
        {
            try
            {
                var senderUser = await userConverterService.GetUserAsync(
                    RequestInfo.Empty with { UserId = item.OwnerPeer.PeerId },
                    item.SenderPeer.PeerId,
                    skipSetContactProperties: true,
                    skipCheckPrivacy: true);
                users.Add(senderUser);
                logger.LogDebug("Loaded sender user {UserId} for inbox message update", item.SenderPeer.PeerId);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to load sender user {UserId} for inbox message update", item.SenderPeer.PeerId);
            }
        }

        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = users,
            Chats = new TVector<IChat>(),
            Date = item.Date,
            Seq = 0
        };
    }

    private async Task<IUpdates> CreateOutboxMessageUpdatesAsync(MessageItem item)
    {
        // Для исходящих сообщений формируем TUpdateNewMessage с out=true
        var message = messageConverterService.ToMessage(item.SenderUserId, item);
        var update = new TUpdateNewMessage
        {
            Message = message,
            Pts = item.Pts,
            PtsCount = 1
        };

        // Подгружаем данные получателя в апдейты (иначе пропадают Premium и статус)
        var users = new TVector<IUser>();
        if (item.ToPeer.PeerType == PeerType.User)
        {
            try
            {
                var recipientUser = await userConverterService.GetUserAsync(
                    RequestInfo.Empty with { UserId = item.OwnerPeer.PeerId },
                    item.ToPeer.PeerId,
                    skipSetContactProperties: true,
                    skipCheckPrivacy: true);
                users.Add(recipientUser);
                logger.LogDebug("Loaded recipient user {UserId} for outbox message update", item.ToPeer.PeerId);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to load recipient user {UserId} for outbox message update", item.ToPeer.PeerId);
            }
        }

        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = users,
            Chats = new TVector<IChat>(),
            Date = item.Date,
            Seq = 0
        };
    }
}