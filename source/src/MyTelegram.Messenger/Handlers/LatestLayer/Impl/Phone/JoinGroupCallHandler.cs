// ReSharper disable All

using MyTelegram.QueryHandlers.MongoDB;

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Messenger.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Join a group call
/// <para>Possible errors</para>
/// Code Type Description
/// 400 DATA_JSON_INVALID The provided JSON data is invalid.
/// 403 GROUPCALL_FORBIDDEN The group call has already ended.
/// 400 GROUPCALL_INVALID The specified group call is invalid.
/// 400 GROUPCALL_SSRC_DUPLICATE_MUCH The app needs to retry joining the group call with a new SSRC value.
/// 400 JOIN_AS_PEER_INVALID The specified peer cannot be used to join a group call.
/// See <a href="https://corefork.telegram.org/method/phone.joinGroupCall" />
///</summary>
internal sealed class JoinGroupCallHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper,
    IGroupCallAppService groupCallAppService,
    IOptions<MyTelegramMessengerServerOptions> options,
    ILogger<JoinGroupCallHandler> _logger,
    ISsrcManagementService ssrcManagementService,
    IGroupCallConnectionService groupCallConnectionService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestJoinGroupCall, MyTelegram.Schema.IUpdates>,
    Phone.IJoinGroupCallHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestJoinGroupCall obj)
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
        
        if (groupCallReadModel!.IsDiscarded)
        {
            RpcErrors.RpcErrors403.GroupcallForbidden.ThrowRpcError();
        }
        
        // Parse WebRTC params from client and extract SSRC
        string? paramsJson = null;
        int ssrc = 0;
        
        try
        {
            if (!string.IsNullOrEmpty(obj.Params?.Data))
            {
                paramsJson = obj.Params.Data;
                var paramsDoc = System.Text.Json.JsonDocument.Parse(paramsJson);
                
                // Extract SSRC from client params
                if (paramsDoc.RootElement.TryGetProperty("ssrc", out var ssrcElement))
                {
                    ssrc = ssrcElement.GetInt32();
                }
            }
        }
        catch
        {
            RpcErrors.RpcErrors400.DataJsonInvalid.ThrowRpcError();
        }
        
        // If client didn't provide SSRC, generate one server-side
        if (ssrc == 0)
        {
            ssrc = await ssrcManagementService.GetOrCreateSsrcAsync(input.UserId, callId);
        }
        
        // Get join-as peer
        var joinAsPeer = peerHelper.GetPeer(obj.JoinAs, input.UserId);
        
        // Check for SSRC duplicate
        if (groupCallReadModel.Participants.Any(p => p.Source == ssrc && !p.Left))
        {
            RpcErrors.RpcErrors400.GroupcallSsrcDuplicateMuch.ThrowRpcError();
        }
        
        var command = new JoinGroupCallCommand(
            GroupCallId.Create(callId),
            input.ToRequestInfo(),
            joinAsPeer.PeerId,
            joinAsPeer.PeerType,
            ssrc,
            obj.Muted,
            obj.VideoStopped,
            paramsJson,
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
        await commandBus.PublishAsync(command, default);
        
        // Build response with updateGroupCallParticipants
        var participant = new TGroupCallParticipant
        {
            Peer = joinAsPeer.PeerType switch
            {
                PeerType.User => new TPeerUser { UserId = joinAsPeer.PeerId },
                PeerType.Chat => new TPeerChat { ChatId = joinAsPeer.PeerId },
                PeerType.Channel => new TPeerChannel { ChannelId = joinAsPeer.PeerId },
                _ => new TPeerUser { UserId = joinAsPeer.PeerId }
            },
            Date = command.Date,
            Source = ssrc,
            Muted = obj.Muted || groupCallReadModel.JoinMuted,
            CanSelfUnmute = true,
            Volume = 10000,
            Self = joinAsPeer.PeerId == input.UserId,
            JustJoined = true
        };
        
        var update = new TUpdateGroupCallParticipants
        {
            Call = obj.Call,
            Participants = new TVector<IGroupCallParticipant>(participant),
            Version = (int)(groupCallReadModel.Version ?? 0) + 1
        };
        
        // Broadcast to all participants (except the one who just joined)
        await groupCallAppService.BroadcastUpdateToParticipantsAsync(
            callId, 
            update, 
            new List<long> { joinAsPeer.PeerId });
        
        // Broadcast group call update to all chat/channel members
        var groupCallForBroadcast = new TGroupCall
        {
            Id = callId,
            AccessHash = groupCallReadModel.AccessHash,
            ParticipantsCount = groupCallReadModel.Participants.Count(p => !p.Left) + 1,
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
        
        // Generate WebRTC connection parameters using Mediasoup SFU format
        var webRtcParams = await groupCallConnectionService.GenerateConnectionJsonAsync(
            input.UserId, 
            callId, 
            ssrc,
            paramsJson);
        
        var connectionUpdate = new TUpdateGroupCallConnection
        {
            Presentation = false,
            Params = new TDataJSON
            {
                Data = webRtcParams
            }
        };
        
        _logger.LogInformation("🔌 Sending TUpdateGroupCallConnection to user {UserId} for call {CallId}, SSRC={Ssrc}", 
            input.UserId, callId, ssrc);
        _logger.LogInformation("📦 FULL Connection JSON: {Params}", webRtcParams);
        
        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update, connectionUpdate),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = command.Date,
            Seq = 0
        };
    }
    
}
