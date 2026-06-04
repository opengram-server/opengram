// ReSharper disable All

using MyTelegram.QueryHandlers.MongoDB;

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Get info about a group call
/// <para>Possible errors</para>
/// Code Type Description
/// 403 GROUPCALL_FORBIDDEN The group call has already ended.
/// 400 GROUPCALL_INVALID The specified group call is invalid.
/// See <a href="https://corefork.telegram.org/method/phone.getGroupCall" />
///</summary>
internal sealed class GetGroupCallHandler(
    IQueryProcessor queryProcessor,
    IAccessHashHelper accessHashHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestGetGroupCall, MyTelegram.Schema.Phone.IGroupCall>,
    Phone.IGetGroupCallHandler
{
    protected override async Task<MyTelegram.Schema.Phone.IGroupCall> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestGetGroupCall obj)
    {
        // Validate access hash
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
        
        // Check if call is discarded
        if (groupCallReadModel!.IsDiscarded)
        {
            RpcErrors.RpcErrors403.GroupcallForbidden.ThrowRpcError();
        }
        
        // Validate access hash
        if (obj.Call is TInputGroupCall inputCall && inputCall.AccessHash != groupCallReadModel.AccessHash)
        {
            RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
        }
        
        // Build GroupCall object
        var groupCall = new TGroupCall
        {
            Id = groupCallReadModel.CallId,
            AccessHash = groupCallReadModel.AccessHash,
            ParticipantsCount = groupCallReadModel.ParticipantsCount,
            Title = groupCallReadModel.Title,
            Version = (int)(groupCallReadModel.Version ?? 0),
            JoinMuted = groupCallReadModel.JoinMuted,
            CanChangeJoinMuted = groupCallReadModel.CanChangeJoinMuted,
            CanStartVideo = groupCallReadModel.CanStartVideo,
            RecordVideoActive = groupCallReadModel.RecordVideoActive,
            RtmpStream = groupCallReadModel.RtmpStream,
            StreamDcId = groupCallReadModel.StreamDcId,
            RecordStartDate = groupCallReadModel.RecordStartDate,
            ScheduleDate = groupCallReadModel.ScheduleDate,
            UnmutedVideoCount = groupCallReadModel.UnmutedVideoCount,
            UnmutedVideoLimit = groupCallReadModel.UnmutedVideoLimit
        };
        
        // Get participants (with pagination)
        var limit = Math.Min(obj.Limit, 100); // Cap at 100
        var activeParticipants = groupCallReadModel.Participants
            .Where(p => !p.Left)
            .OrderByDescending(p => p.JoinedDate)
            .Take(limit)
            .ToList();
        
        var participants = new List<IGroupCallParticipant>();
        foreach (var p in activeParticipants)
        {
            var participant = new TGroupCallParticipant
            {
                Peer = p.PeerType switch
                {
                    "User" => new TPeerUser { UserId = p.PeerId },
                    "Chat" => new TPeerChat { ChatId = p.PeerId },
                    "Channel" => new TPeerChannel { ChannelId = p.PeerId },
                    _ => new TPeerUser { UserId = p.PeerId }
                },
                Date = p.JoinedDate,
                ActiveDate = p.ActiveDate,
                Source = p.Source,
                Muted = p.Muted,
                CanSelfUnmute = p.CanSelfUnmute,
                Volume = p.Volume,
                About = p.About,
                RaiseHandRating = p.RaiseHandRating
            };
            participants.Add(participant);
        }
        
        // Calculate next offset for pagination
        var nextOffset = activeParticipants.Count >= limit 
            ? activeParticipants.Last().JoinedDate.ToString() 
            : string.Empty;
        
        return new MyTelegram.Schema.Phone.TGroupCall
        {
            Call = groupCall,
            Participants = new TVector<IGroupCallParticipant>(participants),
            ParticipantsNextOffset = nextOffset,
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>()
        };
    }
}

// GetGroupCallByIdQuery moved to GroupCallQueryHandler.cs
