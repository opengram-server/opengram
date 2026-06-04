using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;
using MyTelegram.QueryHandlers.MongoDB;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Start or stop recording a group call: the recorded audio and video streams will be automatically sent to <code>Saved messages</code> (the chat with ourselves).
/// <para>Possible errors</para>
/// Code Type Description
/// 403 GROUPCALL_FORBIDDEN The group call has already ended.
/// 400 GROUPCALL_NOT_MODIFIED Group call settings weren't modified.
/// See <a href="https://corefork.telegram.org/method/phone.toggleGroupCallRecord" />
///</summary>
internal sealed class ToggleGroupCallRecordHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IGroupCallAppService groupCallAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestToggleGroupCallRecord, MyTelegram.Schema.IUpdates>,
    Phone.IToggleGroupCallRecordHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestToggleGroupCallRecord obj)
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

        var command = new ToggleGroupCallRecordCommand(
            GroupCallId.Create(callId),
            input.ToRequestInfo(),
            obj.Start,
            obj.Video,
            obj.Title,
            obj.VideoPortrait);

        await commandBus.PublishAsync(command, default);

        var update = new TUpdateGroupCall
        {
            Call = new TGroupCall
            {
                Id = callId,
                AccessHash = groupCallReadModel.AccessHash,
                ParticipantsCount = groupCallReadModel.ParticipantsCount,
                Title = groupCallReadModel.Title,
                StreamDcId = groupCallReadModel.StreamDcId,
                RecordStartDate = obj.Start ? (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() : null,
                RecordVideoActive = obj.Video,
                JoinMuted = groupCallReadModel.JoinMuted,
                CanChangeJoinMuted = groupCallReadModel.CanChangeJoinMuted,
                CanStartVideo = groupCallReadModel.CanStartVideo,
                RtmpStream = groupCallReadModel.RtmpStream,
                UnmutedVideoCount = groupCallReadModel.UnmutedVideoCount,
                UnmutedVideoLimit = groupCallReadModel.UnmutedVideoLimit,
                Version = (int)(groupCallReadModel.Version ?? 0) + 1
            },
            ChatId = groupCallReadModel.PeerId
        };

        // Broadcast update to all participants
        await groupCallAppService.BroadcastUpdateToParticipantsAsync(
            callId,
            update);

        return new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Seq = 0
        };
    }
}
