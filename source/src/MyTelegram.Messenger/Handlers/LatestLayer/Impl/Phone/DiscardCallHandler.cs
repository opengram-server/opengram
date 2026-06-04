// ReSharper disable All

using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;
using MyTelegram.Domain.Services;
using MyTelegram.Services;
using MyTelegram.Messenger.Converters.ConverterServices;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Refuse or end running call
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CALL_ALREADY_ACCEPTED The call was already accepted.
/// 500 CALL_OCCUPY_FAILED The call failed because the user is already making another call.
/// 400 CALL_PEER_INVALID The provided call peer object is invalid.
/// See <a href="https://corefork.telegram.org/method/phone.discardCall" />
///</summary>
internal sealed class DiscardCallHandler(
    ICommandBus commandBus,
    IAggregateStore aggregateStore,
    IUserAppService userAppService,
    IUserConverterService userConverterService,
    IPhotoAppService photoAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestDiscardCall, MyTelegram.Schema.IUpdates>,
    Phone.IDiscardCallHandler
{
    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestDiscardCall obj)
    {
        var reason = MapDiscardReason(obj.Reason);
        var callId = PhoneCallId.Create(obj.Peer.Id);
        
        // Load aggregate to get AdminId and ParticipantId
        var callAggregate = await aggregateStore.LoadAsync<PhoneCallAggregate, PhoneCallId>(callId, default);
        
        var command = new DiscardCallCommand(
            callId,
            input.ToRequestInfo(),
            reason,
            obj.Duration,
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            obj.Video);
        
        await commandBus.PublishAsync(command, default);
        
        // Send update to all participants
        var phoneCallDiscarded = new TPhoneCallDiscarded
        {
            Id = obj.Peer.Id,
            Reason = obj.Reason,
            Duration = obj.Duration,
            Video = obj.Video,
            NeedRating = true,
            NeedDebug = false
        };
        
        var update = new TUpdatePhoneCall
        {
            PhoneCall = phoneCallDiscarded
        };
        
        // Get user info for response if call exists
        var users = new TVector<IUser>();
        if (callAggregate.Version > 0)
        {
            var adminId = callAggregate.State.AdminId;
            var participantId = callAggregate.State.ParticipantId;
            users = await GetUsersAsync(input, adminId, participantId);
        }
        
        return new TUpdates
        {
            Updates = new TVector<IUpdate> { update },
            Users = users,
            Chats = new TVector<IChat>(),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Seq = 0
        };
    }
    
    private static PhoneCallDiscardReason MapDiscardReason(IPhoneCallDiscardReason? tlReason)
    {
        if (tlReason == null)
            return PhoneCallDiscardReason.Hangup;
        
        return tlReason switch
        {
            TPhoneCallDiscardReasonMissed => PhoneCallDiscardReason.Missed,
            TPhoneCallDiscardReasonDisconnect => PhoneCallDiscardReason.Disconnected,
            TPhoneCallDiscardReasonHangup => PhoneCallDiscardReason.Hangup,
            TPhoneCallDiscardReasonBusy => PhoneCallDiscardReason.Busy,
            _ => PhoneCallDiscardReason.Hangup
        };
    }
    
    private async Task<TVector<IUser>> GetUsersAsync(IRequestInput input, long userId1, long userId2)
    {
        var userIds = new List<long> { userId1, userId2 };
        var userReadModels = await userAppService.GetListAsync(userIds);
        
        var users = new List<IUser>();
        foreach (var userReadModel in userReadModels)
        {
            var photos = await photoAppService.GetPhotosAsync(userReadModel);
            var user = userConverterService.ToUser(input, userReadModel, photos, layer: input.Layer);
            users.Add(user);
        }
        
        return new TVector<IUser>(users);
    }
}
