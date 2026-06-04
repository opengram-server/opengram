// ReSharper disable All

using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Commands.GroupCall;
using MyTelegram.QueryHandlers.MongoDB;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Start a scheduled group call.
/// See <a href="https://corefork.telegram.org/method/phone.startScheduledGroupCall" />
///</summary>
internal sealed class StartScheduledGroupCallHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor) 
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestStartScheduledGroupCall, MyTelegram.Schema.IUpdates>,
    Phone.IStartScheduledGroupCallHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestStartScheduledGroupCall obj)
    {
        // Get call ID
        long callId;
        long accessHash;
        if (obj.Call is TInputGroupCall inputGroupCall)
        {
            callId = inputGroupCall.Id;
            accessHash = inputGroupCall.AccessHash;
        }
        else
        {
            RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
            return null!;
        }

        // Get group call to verify it exists and is scheduled
        var groupCallReadModel = await queryProcessor.ProcessAsync(
            new GetGroupCallByIdQuery(GroupCallId.Create(callId).Value),
            default);

        if (groupCallReadModel == null)
        {
            RpcErrors.RpcErrors400.GroupcallInvalid.ThrowRpcError();
            return null!;
        }

        if (!groupCallReadModel.ScheduleDate.HasValue)
        {
            RpcErrorsCustom.RpcErrors400.GroupcallNotScheduled.ThrowRpcError();
            return null!;
        }

        // Create command to start the scheduled call
        var command = new StartScheduledGroupCallCommand(
            GroupCallId.Create(callId),
            input.ToRequestInfo(),
            date: (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        await commandBus.PublishAsync(command, default);

        // Return empty updates for now
        // The group call update will be sent via push notifications to participants
        return new TUpdates
        {
            Updates = new TVector<IUpdate>(),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Seq = 0
        };
    }
}
