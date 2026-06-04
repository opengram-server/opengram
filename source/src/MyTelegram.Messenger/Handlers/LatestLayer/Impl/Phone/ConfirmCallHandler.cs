// ReSharper disable All

using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Commands.PhoneCall;
using MyTelegram.Domain.Services;
using MyTelegram.Domain.Shared;
using Microsoft.Extensions.Options;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Phone;

///<summary>
/// <a href="https://corefork.telegram.org/api/end-to-end/voice-calls">Complete phone call E2E encryption key exchange »</a>
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CALL_ALREADY_DECLINED The call was already declined.
/// 400 CALL_PEER_INVALID The provided call peer object is invalid.
/// See <a href="https://corefork.telegram.org/method/phone.confirmCall" />
///</summary>
internal sealed class ConfirmCallHandler(
    ICommandBus commandBus,
    IDiffieHellmanService dhService,
    IOptions<MyTelegramMessengerServerOptions> options,
    IAggregateStore aggregateStore,
    IUserAppService userAppService,
    IUserConverterService userConverterService,
    IPhotoAppService photoAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Phone.RequestConfirmCall, MyTelegram.Schema.Phone.IPhoneCall>,
    Phone.IConfirmCallHandler
{
    protected override async Task<MyTelegram.Schema.Phone.IPhoneCall> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Phone.RequestConfirmCall obj)
    {
        Console.WriteLine($"[DEBUG ConfirmCallHandler] ConfirmCall invoked: CallId={obj.Peer.Id}, UserId={input.UserId}");
        
        // Validate g_a (must be 256 bytes)
        if (obj.GA == null || obj.GA.Length != 256)
        {
            RpcErrors.RpcErrors400.CallProtocolFlagsInvalid.ThrowRpcError();
        }
        
        var dhConfig = dhService.GetDhConfig();
        
        // Validate g_a
        if (!dhService.ValidateDhValue(obj.GA, dhConfig))
        {
            RpcErrors.RpcErrors400.CallProtocolFlagsInvalid.ThrowRpcError();
        }
        
        // Get call aggregate to retrieve AdminId, ParticipantId, and Video
        var callId = PhoneCallId.Create(obj.Peer.Id);
        var callAggregate = await aggregateStore.LoadAsync<PhoneCallAggregate, PhoneCallId>(callId, default);
        
        if (callAggregate.Version == 0)
        {
            RpcErrors.RpcErrors400.CallPeerInvalid.ThrowRpcError();
        }
        
        var adminId = callAggregate.State.AdminId;
        var participantId = callAggregate.State.ParticipantId;
        var isVideo = callAggregate.State.IsVideo;
        var accessHash = callAggregate.State.AccessHash; // Use AccessHash from state!
        
        // Get WebRTC connections from config
        var connections = GetWebRtcConnections(options.Value);
        
        var protocol = MapProtocol(obj.Protocol);
        
        var command = new ConfirmCallCommand(
            callId,
            input.ToRequestInfo(),
            obj.GA,
            obj.KeyFingerprint,
            protocol,
            connections,
            (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
        await commandBus.PublishAsync(command, default);
        
        // Build WebRTC connections for response
        var tlConnections = connections.Select(c => new TPhoneConnectionWebrtc
        {
            Id = c.Id,
            Ip = c.Ip,
            Ipv6 = c.Ipv6,
            Port = c.Port,
            Turn = c.IsTurn,
            Stun = c.IsStun,
            Username = c.Username,
            Password = c.Password
        }).ToList();
        
        var phoneCall = new TPhoneCall
        {
            Id = obj.Peer.Id,
            AccessHash = accessHash, // Use stored AccessHash, not from request
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            AdminId = adminId,
            ParticipantId = participantId,
            GAOrB = obj.GA,
            KeyFingerprint = obj.KeyFingerprint,
            Protocol = obj.Protocol,
            Connections = new TVector<IPhoneConnection>(tlConnections),
            StartDate = command.StartDate,
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
            PhoneCall = phoneCall,
            Users = users
        };
    }
    
    private static List<PhoneConnectionInfo> GetWebRtcConnections(MyTelegramMessengerServerOptions options)
    {
        var connections = new List<PhoneConnectionInfo>();
        
        // Add configured WebRTC/TURN/STUN servers
        if (options.WebRtcConnections != null && options.WebRtcConnections.Any())
        {
            foreach (var conn in options.WebRtcConnections)
            {
                var peerTag = new byte[16];
                Random.Shared.NextBytes(peerTag);
                
                connections.Add(new PhoneConnectionInfo(
                    Id: Random.Shared.NextInt64(),
                    Ip: conn.Ip ?? "0.0.0.0",
                    Ipv6: conn.Ipv6 ?? "::",
                    Port: conn.Port,
                    PeerTag: peerTag,
                    IsTurn: conn.Turn,
                    IsStun: conn.Stun,
                    Username: conn.UserName,
                    Password: conn.Password));
            }
        }
        else
        {
            // Fallback: Add empty connection for P2P mode when TURN is not configured
            // Telegram Desktop will use direct P2P connection based on Protocol.UdpP2p
            var peerTag = new byte[16];
            Random.Shared.NextBytes(peerTag);
            
            connections.Add(new PhoneConnectionInfo(
                Id: Random.Shared.NextInt64(),
                Ip: "0.0.0.0",
                Ipv6: "::",
                Port: 0,
                PeerTag: peerTag,
                IsTurn: false,
                IsStun: false,
                Username: "",
                Password: ""));
        }
        
        return connections;
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
