using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Queries;
using MyTelegram.QueryHandlers.MongoDB;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.Messenger.Services;

public class GroupCallAppService(
    IQueryProcessor queryProcessor,
    IObjectMessageSender objectMessageSender,
    IChannelAppService channelAppService,
    ILogger<GroupCallAppService> logger) : IGroupCallAppService, ITransientDependency
{
    public async Task<bool> IsAdminAsync(long userId, long peerId, PeerType peerType)
    {
        try
        {
            switch (peerType)
            {
                case PeerType.Chat:
                    var chatReadModel = await queryProcessor.ProcessAsync(new GetChatByChatIdQuery(peerId), default);
                    // In regular chats, the creator is the admin
                    return (chatReadModel as ChatReadModel)?.CreatorId == userId;
                
                case PeerType.Channel:
                    var channelMember = await queryProcessor.ProcessAsync(
                        new GetChannelMemberByUserIdQuery(peerId, userId), default);
                    return channelMember?.IsAdmin ?? false;
                
                default:
                    return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking admin rights for userId={UserId}, peerId={PeerId}, peerType={PeerType}", 
                userId, peerId, peerType);
            return false;
        }
    }

    public async Task<bool> PeerExistsAsync(long peerId, PeerType peerType)
    {
        try
        {
            switch (peerType)
            {
                case PeerType.Chat:
                    var chat = await queryProcessor.ProcessAsync(new GetChatByChatIdQuery(peerId), default);
                    return chat != null;
                
                case PeerType.Channel:
                    var channel = await channelAppService.GetAsync(peerId);
                    return channel != null;
                
                default:
                    return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking peer existence for peerId={PeerId}, peerType={PeerType}", 
                peerId, peerType);
            return false;
        }
    }

    public async Task BroadcastUpdateToParticipantsAsync(long callId, IUpdate update, List<long>? excludeUserIds = null)
    {
        try
        {
            var groupCall = await queryProcessor.ProcessAsync(
                new GetGroupCallByIdQuery(GroupCallId.Create(callId).Value), default);
            
            if (groupCall == null)
            {
                logger.LogWarning("Group call not found for broadcasting: {CallId}", callId);
                return;
            }
            
            var participantUserIds = groupCall.Participants
                .Where(p => !p.Left && p.PeerType == "User")
                .Select(p => p.PeerId)
                .ToList();
            
            if (excludeUserIds?.Any() == true)
            {
                participantUserIds = participantUserIds.Except(excludeUserIds).ToList();
            }
            
            var updates = new TUpdates
            {
                Updates = new TVector<IUpdate>(update),
                Users = new TVector<IUser>(),
                Chats = new TVector<IChat>(),
                Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Seq = 0
            };
            
            foreach (var userId in participantUserIds)
            {
                try
                {
                    await objectMessageSender.PushMessageToPeerAsync(new Peer(PeerType.User, userId), updates, null);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error broadcasting update to userId={UserId}", userId);
                }
            }
            
            logger.LogInformation("Broadcast update to {Count} participants for call {CallId}", 
                participantUserIds.Count, callId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error broadcasting update for callId={CallId}", callId);
        }
    }

    public async Task BroadcastGroupCallUpdateAsync(long peerId, PeerType peerType, IGroupCall groupCall)
    {
        try
        {
            var update = new TUpdateGroupCall
            {
                ChatId = peerId,
                Call = groupCall
            };
            
            List<long> memberUserIds = new();
            
            switch (peerType)
            {
                case PeerType.Chat:
                    var chatReadModel = await queryProcessor.ProcessAsync(new GetChatByChatIdQuery(peerId), default);
                    if (chatReadModel is ChatReadModel chat && chat.ChatMembers != null)
                    {
                        memberUserIds = chat.ChatMembers.Select(m => m.UserId).ToList();
                    }
                    break;
                
                case PeerType.Channel:
                    // Get first 1000 channel members (simplified)
                    var channelMembers = await queryProcessor.ProcessAsync(
                        new GetChannelMembersByChannelIdQuery(peerId, new List<long>(), 0, 1000, false, false, false, false, null), default);
                    if (channelMembers != null)
                    {
                        memberUserIds = channelMembers.Select(m => m.UserId).ToList();
                    }
                    break;
            }
            
            var updatesList = new List<IUpdate> { update };
            
            // For channels, also send updateChannel to trigger UI updates
            if (peerType == PeerType.Channel)
            {
                updatesList.Add(new TUpdateChannel
                {
                    ChannelId = peerId
                });
            }
            
            var updates = new TUpdates
            {
                Updates = new TVector<IUpdate>(updatesList.ToArray()),
                Users = new TVector<IUser>(),
                Chats = new TVector<IChat>(),
                Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Seq = 0
            };
            
            foreach (var userId in memberUserIds)
            {
                try
                {
                    await objectMessageSender.PushMessageToPeerAsync(new Peer(PeerType.User, userId), updates, null);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error broadcasting group call update to userId={UserId}", userId);
                }
            }
            
            logger.LogInformation("Broadcast group call update to {Count} members for peerId={PeerId}", 
                memberUserIds.Count, peerId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error broadcasting group call update for peerId={PeerId}", peerId);
        }
    }
}
