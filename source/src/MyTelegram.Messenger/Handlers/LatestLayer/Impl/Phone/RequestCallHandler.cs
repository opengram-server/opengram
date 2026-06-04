// ReSharper disable All

using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;
using MyTelegram.Domain.Services;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Start a telegram phone call
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CALL_PROTOCOL_FLAGS_INVALID Call protocol flags invalid.
/// 400 INPUT_USER_DEACTIVATED The specified user was deleted.
/// 400 PARTICIPANT_VERSION_OUTDATED The other participant does not use an up to date telegram client with support for calls.
/// 400 USER_ID_INVALID The provided user ID is invalid.
/// 403 USER_IS_BLOCKED You were blocked by this user.
/// 403 USER_PRIVACY_RESTRICTED The user's privacy settings do not allow you to do this.
/// See <a href="https://corefork.telegram.org/method/phone.requestCall" />
///</summary>
internal sealed class RequestCallHandler(
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAccessHashHelper accessHashHelper,
    IPeerHelper peerHelper,
    IUserAppService userAppService,
    IUserConverterService userConverterService,
    IPhotoAppService photoAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestRequestCall, MyTelegram.Schema.Phone.IPhoneCall>,
    Phone.IRequestCallHandler
{
    protected override async Task<MyTelegram.Schema.Phone.IPhoneCall> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestRequestCall obj)
    {
        Console.WriteLine($"[DEBUG RequestCallHandler] RequestCall invoked: UserId={input.UserId} → Target={obj.UserId}");
        
        // Validate input user
        await accessHashHelper.CheckAccessHashAsync(input, obj.UserId);
        
        var toPeer = peerHelper.GetPeer(obj.UserId, input.UserId);
        var toUserId = toPeer.PeerId;
        
        // Cannot call yourself
        if (toUserId == input.UserId)
        {
            RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
        }
        
        // Generate unique call ID
        var callId = await idGenerator.NextLongIdAsync(IdType.MessageId);
        var accessHash = Random.Shared.NextInt64();
        
        // Validate g_a_hash (must be 32 bytes - SHA256)
        if (obj.GAHash == null || obj.GAHash.Length != 32)
        {
            RpcErrors.RpcErrors400.CallProtocolFlagsInvalid.ThrowRpcError();
        }
        
        // Map protocol from TL schema to domain
        var protocol = MapProtocol(obj.Protocol);
        
        var command = new RequestCallCommand(
            PhoneCallId.Create(callId),
            input.ToRequestInfo(),
            callId,
            accessHash,
            input.UserId,
            toUserId,
            obj.Video,
            obj.GAHash,
            protocol,
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
        await commandBus.PublishAsync(command, default);
        
        // Build response
        var phoneCallRequested = new TPhoneCallRequested
        {
            Id = callId,
            AccessHash = accessHash,
            Date = command.Date,
            AdminId = input.UserId,
            ParticipantId = toUserId,
            GAHash = obj.GAHash,
            Protocol = obj.Protocol,
            Video = obj.Video
        };
        
        // Get user info for response - MUST contain both participants
        var users = await GetUsersAsync(input, input.UserId, toUserId);
        
        // Validate that we have both users (client requirement)
        if (users.Count < 2)
        {
            // Missing user data - this will cause CLIENT_RESPONSE_PARSE_failed
            RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
        }
        
        return new MyTelegram.Schema.Phone.TPhoneCall
        {
            PhoneCall = phoneCallRequested,
            Users = users
        };
    }
    
    private static PhoneCallProtocol MapProtocol(IPhoneCallProtocol tlProtocol)
    {
        return new PhoneCallProtocol(
            tlProtocol.UdpP2p,
            tlProtocol.UdpReflector,
            tlProtocol.MinLayer,
            tlProtocol.MaxLayer,
            tlProtocol.LibraryVersions?.ToList() ?? new List<string>());
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
