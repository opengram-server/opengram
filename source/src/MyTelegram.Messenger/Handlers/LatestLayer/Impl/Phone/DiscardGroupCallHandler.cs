// ReSharper disable All

using MyTelegram.QueryHandlers.MongoDB;

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Terminate a group call
/// <para>Possible errors</para>
/// Code Type Description
/// 400 GROUPCALL_ALREADY_DISCARDED The group call was already discarded.
/// 403 GROUPCALL_FORBIDDEN The group call has already ended.
/// 400 GROUPCALL_INVALID The specified group call is invalid.
/// See <a href="https://corefork.telegram.org/method/phone.discardGroupCall" />
///</summary>
internal sealed class DiscardGroupCallHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    IGroupCallAppService groupCallAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestDiscardGroupCall, MyTelegram.Schema.IUpdates>,
    Phone.IDiscardGroupCallHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestDiscardGroupCall obj)
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
        
        // Check if user is admin
        var peerType = Enum.Parse<PeerType>(groupCallReadModel.PeerType);
        if (!await groupCallAppService.IsAdminAsync(input.UserId, groupCallReadModel.PeerId, peerType))
        {
            RpcErrors.RpcErrors400.ChatAdminRequired.ThrowRpcError();
        }
        
        var command = new DiscardGroupCallCommand(
            GroupCallId.Create(callId),
            input.ToRequestInfo(),
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
        await commandBus.PublishAsync(command, default);
        
        // Build response with updateGroupCall showing discarded state
        var groupCallDiscarded = new TGroupCallDiscarded
        {
            Id = callId,
            AccessHash = groupCallReadModel.AccessHash,
            Duration = command.Date - groupCallReadModel.CreatedDate
        };
        
        var update = new TUpdateGroupCall
        {
            ChatId = groupCallReadModel.PeerId,
            Call = groupCallDiscarded
        };
        
        // Broadcast to all chat/channel members
        await groupCallAppService.BroadcastGroupCallUpdateAsync(
            groupCallReadModel.PeerId, 
            peerType, 
            groupCallDiscarded);
        
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
