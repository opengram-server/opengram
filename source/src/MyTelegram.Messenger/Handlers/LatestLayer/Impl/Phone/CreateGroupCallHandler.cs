// ReSharper disable All

using MyTelegram.QueryHandlers.MongoDB;

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;
using MyTelegram.Domain.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Create a group call or livestream
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHANNEL_PRIVATE You haven't joined this channel/supergroup.
/// 400 CHAT_ADMIN_REQUIRED You must be an admin in this chat to do this.
/// 400 CREATE_CALL_FAILED An error occurred while creating the call.
/// 400 PEER_ID_INVALID The provided peer id is invalid.
/// 400 SCHEDULE_DATE_INVALID Invalid schedule date provided.
/// See <a href="https://corefork.telegram.org/method/phone.createGroupCall" />
///</summary>
internal sealed class CreateGroupCallHandler(
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IPeerHelper peerHelper,
    IAccessHashHelper accessHashHelper,
    IGroupCallAppService groupCallAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestCreateGroupCall, MyTelegram.Schema.IUpdates>,
    Phone.ICreateGroupCallHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestCreateGroupCall obj)
    {
        // Validate peer
        var peer = peerHelper.GetPeer(obj.Peer, input.UserId);
        
        // Validate peer type (only Chat or Channel)
        if (peer.PeerType == PeerType.User)
        {
            RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
        }
        
        // Generate unique call ID and access hash
        var callId = await idGenerator.NextLongIdAsync(IdType.MessageId);
        var accessHash = Random.Shared.NextInt64();
        
        // Check if peer exists
        if (!await groupCallAppService.PeerExistsAsync(peer.PeerId, peer.PeerType))
        {
            RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
        }
        
        // Check if user is admin
        if (!await groupCallAppService.IsAdminAsync(input.UserId, peer.PeerId, peer.PeerType))
        {
            RpcErrors.RpcErrors400.ChatAdminRequired.ThrowRpcError();
        }
        
        // Validate schedule date
        if (obj.ScheduleDate.HasValue && obj.ScheduleDate.Value <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            RpcErrors.RpcErrors400.ScheduleDateInvalid.ThrowRpcError();
        }
        
        var command = new CreateGroupCallCommand(
            GroupCallId.Create(callId),
            input.ToRequestInfo(),
            callId,
            accessHash,
            peer.PeerId,
            peer.PeerType,
            obj.Title,
            obj.RtmpStream,
            1, // Default DC ID
            obj.ScheduleDate,
            obj.RandomId,
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
        await commandBus.PublishAsync(command, default);
        
        // Build GroupCall object
        var groupCall = new TGroupCall
        {
            Id = callId,
            AccessHash = accessHash,
            ParticipantsCount = 0,
            Title = obj.Title,
            Version = 1,
            CanChangeJoinMuted = true,
            CanStartVideo = true,
            RtmpStream = obj.RtmpStream,
            ScheduleDate = obj.ScheduleDate,
            UnmutedVideoLimit = 30,
            StreamDcId = 1
        };
        
        // Broadcast updateGroupCall to all chat/channel members
        await groupCallAppService.BroadcastGroupCallUpdateAsync(peer.PeerId, peer.PeerType, groupCall);
        
        // Return response
        var update = new TUpdateGroupCall
        {
            ChatId = peer.PeerId,
            Call = groupCall
        };
        
        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = command.Date,
            Seq = 0
        };
    }
}
