using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;
using MyTelegram.QueryHandlers.MongoDB;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Edit the title of a group call or livestream
/// <para>Possible errors</para>
/// Code Type Description
/// 403 GROUPCALL_FORBIDDEN The group call has already ended.
/// See <a href="https://corefork.telegram.org/method/phone.editGroupCallTitle" />
///</summary>
internal sealed class EditGroupCallTitleHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IGroupCallAppService groupCallAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestEditGroupCallTitle, MyTelegram.Schema.IUpdates>,
    Phone.IEditGroupCallTitleHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestEditGroupCallTitle obj)
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

        var command = new EditGroupCallTitleCommand(
            GroupCallId.Create(callId),
            input.ToRequestInfo(),
            obj.Title);

        await commandBus.PublishAsync(command, default);

        var update = new TUpdateGroupCall
        {
            Call = new TGroupCall
            {
                Id = callId,
                AccessHash = groupCallReadModel.AccessHash,
                ParticipantsCount = groupCallReadModel.ParticipantsCount,
                Title = obj.Title,
                StreamDcId = groupCallReadModel.StreamDcId,
                RecordStartDate = groupCallReadModel.RecordStartDate,
                RecordVideoActive = groupCallReadModel.RecordVideoActive,
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
