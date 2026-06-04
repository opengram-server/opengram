// ReSharper disable All

using MyTelegram.QueryHandlers.MongoDB;

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Leave a group call
/// See <a href="https://corefork.telegram.org/method/phone.leaveGroupCall" />
///</summary>
internal sealed class LeaveGroupCallHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IGroupCallAppService groupCallAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestLeaveGroupCall, MyTelegram.Schema.IUpdates>,
    Phone.ILeaveGroupCallHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestLeaveGroupCall obj)
    {
        // Get call ID
        long callId;

        if (obj.Call is TInputGroupCall inputGroupCall)

        {

            callId = inputGroupCall.Id;

        }

        else

        {

            RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();

            return null!;

        }
        
        // Get group call from read model
        var groupCallReadModel = await queryProcessor.ProcessAsync(
            new GetGroupCallByIdQuery(GroupCallId.Create(callId).Value), 
            default);
        
        if (groupCallReadModel == null)
        {
            RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
        }
        
        // Find participant by source (SSRC)
        var participant = groupCallReadModel!.Participants
            .FirstOrDefault(p => p.Source == obj.Source && !p.Left);
        
        if (participant == null)
        {
            RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
        }
        
        var command = new LeaveGroupCallCommand(
            GroupCallId.Create(callId),
            input.ToRequestInfo(),
            participant!.PeerId,
            obj.Source,
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
        await commandBus.PublishAsync(command, default);
        
        // Build response with updateGroupCallParticipants
        var participantUpdate = new TGroupCallParticipant
        {
            Peer = participant.PeerType switch
            {
                "User" => new TPeerUser { UserId = participant.PeerId },
                "Chat" => new TPeerChat { ChatId = participant.PeerId },
                "Channel" => new TPeerChannel { ChannelId = participant.PeerId },
                _ => new TPeerUser { UserId = participant.PeerId }
            },
            Date = participant.JoinedDate,
            Source = participant.Source,
            Left = true
        };
        
        var update = new TUpdateGroupCallParticipants
        {
            Call = obj.Call,
            Participants = new TVector<IGroupCallParticipant>(participantUpdate),
            Version = (int)(groupCallReadModel.Version ?? 0) + 1
        };
        
        // Broadcast to all participants EXCEPT the one who left (they'll get it in the response)
        await groupCallAppService.BroadcastUpdateToParticipantsAsync(
            callId, 
            update, 
            new List<long> { participant.PeerId });
        
        // Broadcast group call update to all chat/channel members
        var remainingParticipantsCount = groupCallReadModel.Participants.Count(p => !p.Left && p.PeerId != participant.PeerId);
        
        // Update group call with new participant count (don't auto-discard even if 0 participants)
        var groupCallForBroadcast = new TGroupCall
        {
            Id = callId,
            AccessHash = groupCallReadModel.AccessHash,
            ParticipantsCount = remainingParticipantsCount,
            Title = groupCallReadModel.Title,
            StreamDcId = groupCallReadModel.StreamDcId,
            RecordStartDate = groupCallReadModel.RecordStartDate,
            ScheduleDate = groupCallReadModel.ScheduleDate,
            UnmutedVideoLimit = 100,
            JoinMuted = groupCallReadModel.JoinMuted,
            CanChangeJoinMuted = await groupCallAppService.IsAdminAsync(input.UserId, groupCallReadModel.PeerId, 
                Enum.Parse<PeerType>(groupCallReadModel.PeerType))
        };
        
        await groupCallAppService.BroadcastGroupCallUpdateAsync(
            groupCallReadModel.PeerId, 
            Enum.Parse<PeerType>(groupCallReadModel.PeerType), 
            groupCallForBroadcast);
        
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
