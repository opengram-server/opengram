// ReSharper disable All

using MyTelegram.QueryHandlers.MongoDB;

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;
using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Edit information about a given group call participantNote: <a href="https://corefork.telegram.org/mtproto/TL-combinators#conditional-fields">flags</a>.N?<a href="https://corefork.telegram.org/type/Bool">Bool</a> parameters can have three possible values:
/// <para>Possible errors</para>
/// Code Type Description
/// 403 GROUPCALL_FORBIDDEN The group call has already ended.
/// 400 PARTICIPANT_JOIN_MISSING Trying to enable a presentation, when the user hasn't joined the Video Chat with <a href="https://corefork.telegram.org/method/phone.joinGroupCall">phone.joinGroupCall</a>.
/// 400 USER_VOLUME_INVALID The specified user volume is invalid.
/// See <a href="https://corefork.telegram.org/method/phone.editGroupCallParticipant" />
///</summary>
internal sealed class EditGroupCallParticipantHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IPeerHelper peerHelper,
    IGroupCallAppService groupCallAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestEditGroupCallParticipant, MyTelegram.Schema.IUpdates>,
    Phone.IEditGroupCallParticipantHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestEditGroupCallParticipant obj)
    {
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
        
        var groupCallReadModel = await queryProcessor.ProcessAsync(
            new GetGroupCallByIdQuery(GroupCallId.Create(callId).Value), default);
        
        if (groupCallReadModel == null || groupCallReadModel.IsDiscarded)
        {
            RpcErrors.RpcErrors403.GroupcallForbidden.ThrowRpcError();
        }
        
        var participantPeer = peerHelper.GetPeer(obj.Participant, input.UserId);
        var participant = groupCallReadModel!.Participants
            .FirstOrDefault(p => p.PeerId == participantPeer.PeerId && !p.Left);
        
        if (participant == null)
        {
            RpcErrors.RpcErrors400.ParticipantJoinMissing.ThrowRpcError();
        }
        
        var peerType = Enum.Parse<PeerType>(groupCallReadModel.PeerType);
        var isAdmin = await groupCallAppService.IsAdminAsync(input.UserId, groupCallReadModel.PeerId, peerType);
        
        if (obj.Volume.HasValue && (obj.Volume.Value < 0 || obj.Volume.Value > 20000))
        {
            RpcErrors.RpcErrors400.UserVolumeInvalid.ThrowRpcError();
        }
        
        var command = new UpdateGroupCallParticipantCommand(
            GroupCallId.Create(callId), input.ToRequestInfo(), participantPeer.PeerId,
            obj.Muted, isAdmin && obj.Muted == true, obj.Volume, obj.RaiseHand,
            obj.VideoStopped ?? obj.VideoPaused,
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
        await commandBus.PublishAsync(command, default);
        
        var participantUpdate = new TGroupCallParticipant
        {
            Peer = participantPeer.PeerType switch
            {
                PeerType.User => new TPeerUser { UserId = participantPeer.PeerId },
                PeerType.Chat => new TPeerChat { ChatId = participantPeer.PeerId },
                PeerType.Channel => new TPeerChannel { ChannelId = participantPeer.PeerId },
                _ => new TPeerUser { UserId = participantPeer.PeerId }
            },
            Date = participant!.JoinedDate,
            Source = participant.Source,
            Muted = obj.Muted ?? participant.Muted,
            CanSelfUnmute = obj.Muted == true && isAdmin ? false : participant.CanSelfUnmute,
            Volume = obj.Volume ?? participant.Volume
        };
        
        var update = new TUpdateGroupCallParticipants
        {
            Call = obj.Call,
            Participants = new TVector<IGroupCallParticipant>(participantUpdate),
            Version = (int)(groupCallReadModel.Version ?? 0) + 1
        };
        
        await groupCallAppService.BroadcastUpdateToParticipantsAsync(callId, update, null);
        
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
