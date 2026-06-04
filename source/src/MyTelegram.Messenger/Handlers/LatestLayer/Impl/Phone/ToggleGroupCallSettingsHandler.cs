// ReSharper disable All

using MyTelegram.QueryHandlers.MongoDB;

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;
using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Change group call settings
/// <para>Possible errors</para>
/// Code Type Description
/// 400 GROUPCALL_NOT_MODIFIED Group call settings weren't modified.
/// See <a href="https://corefork.telegram.org/method/phone.toggleGroupCallSettings" />
///</summary>
internal sealed class ToggleGroupCallSettingsHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IGroupCallAppService groupCallAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestToggleGroupCallSettings, MyTelegram.Schema.IUpdates>,
    Phone.IToggleGroupCallSettingsHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestToggleGroupCallSettings obj)
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
        
        var peerType = Enum.Parse<PeerType>(groupCallReadModel!.PeerType);
        if (!await groupCallAppService.IsAdminAsync(input.UserId, groupCallReadModel.PeerId, peerType))
        {
            RpcErrors.RpcErrors400.ChatAdminRequired.ThrowRpcError();
        }
        
        // Extract join_muted from Bool? (handles nullable bool)
        bool? joinMuted = obj.JoinMuted;
        
        var command = new UpdateGroupCallSettingsCommand(
            GroupCallId.Create(callId), input.ToRequestInfo(), joinMuted);
        
        await commandBus.PublishAsync(command, default);
        
        var groupCall = new TGroupCall
        {
            Id = groupCallReadModel.CallId,
            AccessHash = groupCallReadModel.AccessHash,
            ParticipantsCount = groupCallReadModel.ParticipantsCount,
            Title = groupCallReadModel.Title,
            Version = (int)(groupCallReadModel.Version ?? 0) + 1,
            JoinMuted = joinMuted ?? groupCallReadModel.JoinMuted,
            CanChangeJoinMuted = groupCallReadModel.CanChangeJoinMuted,
            CanStartVideo = groupCallReadModel.CanStartVideo,
            UnmutedVideoLimit = groupCallReadModel.UnmutedVideoLimit
        };
        
        await groupCallAppService.BroadcastGroupCallUpdateAsync(groupCallReadModel.PeerId, peerType, groupCall);
        
        return new TUpdates
        {
            Updates = new TVector<IUpdate>(new TUpdateGroupCall { ChatId = groupCallReadModel.PeerId, Call = groupCall }),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Seq = 0
        };
    }
}
