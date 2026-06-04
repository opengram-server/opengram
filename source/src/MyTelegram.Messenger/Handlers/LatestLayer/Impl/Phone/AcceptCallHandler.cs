// ReSharper disable All

using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;
using MyTelegram.Domain.Services;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// Accept incoming call
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CALL_ALREADY_ACCEPTED The call was already accepted.
/// 400 CALL_ALREADY_DECLINED The call was already declined.
/// 500 CALL_OCCUPY_FAILED The call failed because the user is already making another call.
/// 400 CALL_PEER_INVALID The provided call peer object is invalid.
/// 400 CALL_PROTOCOL_FLAGS_INVALID Call protocol flags invalid.
/// See <a href="https://corefork.telegram.org/method/phone.acceptCall" />
///</summary>
internal sealed class AcceptCallHandler(
    ICommandBus commandBus,
    IDiffieHellmanService dhService,
    IAggregateStore aggregateStore,
    IUserAppService userAppService,
    IUserConverterService userConverterService,
    IPhotoAppService photoAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestAcceptCall, MyTelegram.Schema.Phone.IPhoneCall>,
    Phone.IAcceptCallHandler
{
    protected override async Task<MyTelegram.Schema.Phone.IPhoneCall> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestAcceptCall obj)
    {
        // Get call aggregate to retrieve AdminId and ParticipantId
        var callId = PhoneCallId.Create(obj.Peer.Id);
        var callAggregate = await aggregateStore.LoadAsync<PhoneCallAggregate, PhoneCallId>(callId, default);
        
        if (callAggregate.Version == 0)
        {
            RpcErrors.RpcErrors400.CallPeerInvalid.ThrowRpcError();
        }
        
        // Get DH config and generate g_b
        var dhConfig = dhService.GetDhConfig();
        var (b, gB) = dhService.GenerateGAAndA(dhConfig);
        
        // Validate g_b
        if (!dhService.ValidateDhValue(gB, dhConfig))
        {
            RpcErrors.RpcErrors400.CallProtocolFlagsInvalid.ThrowRpcError();
        }
        
        var protocol = MapProtocol(obj.Protocol);
        
        var command = new AcceptCallCommand(
            callId,
            input.ToRequestInfo(),
            gB,
            protocol);
        
        await commandBus.PublishAsync(command, default);
        
        // Get AdminId, ParticipantId, Video, and AccessHash from aggregate state
        var adminId = callAggregate.State.AdminId;
        var participantId = callAggregate.State.ParticipantId;
        var isVideo = callAggregate.State.IsVideo;
        var accessHash = callAggregate.State.AccessHash; // Use AccessHash from state!
        
        Console.WriteLine($"[DEBUG AcceptCallHandler] Sending phoneCallAccepted: CallId={obj.Peer.Id}, GB_length={gB.Length}");
        Console.WriteLine($"[DEBUG AcceptCallHandler] Protocol: UdpP2p={obj.Protocol.UdpP2p}, UdpReflector={obj.Protocol.UdpReflector}, MinLayer={obj.Protocol.MinLayer}, MaxLayer={obj.Protocol.MaxLayer}");
        Console.WriteLine($"[DEBUG AcceptCallHandler] Protocol.LibraryVersions: {string.Join(", ", obj.Protocol.LibraryVersions ?? new TVector<string>())}");
        Console.WriteLine($"[DEBUG AcceptCallHandler] GB (first 32 bytes): {Convert.ToHexString(gB.ToArray().Take(32).ToArray())}");
        
        // Build response (Connections will be sent in ConfirmCall, not here - per Telegram protocol)
        var phoneCallAccepted = new TPhoneCallAccepted
        {
            Id = obj.Peer.Id,
            AccessHash = accessHash, // Use stored AccessHash, not from request
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            AdminId = adminId,
            ParticipantId = participantId,
            GB = gB,
            Protocol = obj.Protocol,
            Video = isVideo
        };
        
        // Get user info for response - MUST contain both participants
        var users = await GetUsersAsync(input, adminId, participantId);
        
        // Validate that we have both users (client requirement)
        if (users.Count < 2)
        {
            // Missing user data - this will cause CLIENT_RESPONSE_PARSE_failed
            RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
        }
        
        return new MyTelegram.Schema.Phone.TPhoneCall
        {
            PhoneCall = phoneCallAccepted,
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
