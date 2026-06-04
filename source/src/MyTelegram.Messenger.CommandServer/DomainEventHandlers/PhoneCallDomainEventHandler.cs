using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Events.PhoneCall;
using MyTelegram.Domain.Shared;
using MyTelegram.Messenger.DomainEventHandlers;
using MyTelegram.Schema;
using MyTelegram.Converters.Extensions;
using MyTelegram.Services;
using MyTelegram.Messenger.Converters.ConverterServices;

namespace MyTelegram.Messenger.CommandServer.DomainEventHandlers;

public class PhoneCallDomainEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService,
    IUserAppService userAppService,
    IUserConverterService userConverterService,
    IPhotoAppService photoAppService)
    : DomainEventHandlerBase(objectMessageSender, commandBus, idGenerator, ackCacheService),
        ISubscribeSynchronousTo<PhoneCallAggregate, PhoneCallId, PhoneCallRequestedEvent>,
        ISubscribeSynchronousTo<PhoneCallAggregate, PhoneCallId, PhoneCallAcceptedEvent>,
        ISubscribeSynchronousTo<PhoneCallAggregate, PhoneCallId, PhoneCallConfirmedEvent>,
        ISubscribeSynchronousTo<PhoneCallAggregate, PhoneCallId, PhoneCallDiscardedEvent>,
        ISubscribeSynchronousTo<PhoneCallAggregate, PhoneCallId, PhoneCallReceivedEvent>
{
    public async Task HandleAsync(
        IDomainEvent<PhoneCallAggregate, PhoneCallId, PhoneCallRequestedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;

        Console.WriteLine($"[DEBUG PhoneCallDomainEventHandler] PhoneCallRequestedEvent: CallId={evt.CallId}, AdminId={evt.AdminId}, ParticipantId={evt.ParticipantId}");

        // Send TPhoneCallRequested to the participant (callee)
        var phoneCallRequested = new TPhoneCallRequested
        {
            Id = evt.CallId,
            AccessHash = evt.AccessHash,
            Date = evt.Date,
            AdminId = evt.AdminId,
            ParticipantId = evt.ParticipantId,
            GAHash = evt.GAHash.ToReadOnlyMemory(),
            Protocol = evt.Protocol.ToPhoneCallProtocol(),
            Video = evt.IsVideo
        };

        var updatePhoneCall = new TUpdatePhoneCall
        {
            PhoneCall = phoneCallRequested
        };

        // Get user info for both participants
        var users = await GetUsersAsync(evt.RequestInfo, evt.AdminId, evt.ParticipantId);
        
        Console.WriteLine($"[DEBUG PhoneCallDomainEventHandler] Loaded {users.Count} users for RequestCall Update");
        
        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate> { updatePhoneCall },
            Users = users,
            Chats = new TVector<IChat>(),
            Date = evt.Date,
            Seq = 0
        };

        // Send to callee (participant) - notify them about incoming call
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, evt.ParticipantId),
            updates,
            updatesType: UpdatesType.Updates
        );

        Console.WriteLine($"[DEBUG PhoneCallDomainEventHandler] Update sent to callee ParticipantId={evt.ParticipantId}");

        // NOTE: Do NOT send RPC response here - RequestCallHandler already returned response to caller
        // The RPC response is handled by RequestCallHandler with proper Users vector
    }

    public async Task HandleAsync(
        IDomainEvent<PhoneCallAggregate, PhoneCallId, PhoneCallAcceptedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;

        Console.WriteLine($"[DEBUG PhoneCallDomainEventHandler] PhoneCallAcceptedEvent: CallId={evt.CallId}, AdminId={evt.AdminId}, ParticipantId={evt.ParticipantId}");

        // Send TPhoneCallAccepted to the admin (caller)
        var phoneCallAccepted = new TPhoneCallAccepted
        {
            Id = evt.CallId,
            AccessHash = evt.AccessHash,
            Date = DateTime.UtcNow.ToTimestamp(),
            AdminId = evt.AdminId,
            ParticipantId = evt.ParticipantId,
            GB = evt.GB.ToReadOnlyMemory(),
            Protocol = evt.Protocol.ToPhoneCallProtocol(),
            Video = evt.IsVideo
        };

        var updatePhoneCall = new TUpdatePhoneCall
        {
            PhoneCall = phoneCallAccepted
        };

        // Get user info for both participants
        var users = await GetUsersAsync(evt.RequestInfo, evt.AdminId, evt.ParticipantId);
        
        Console.WriteLine($"[DEBUG PhoneCallDomainEventHandler] Loaded {users.Count} users for Update");
        
        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate> { updatePhoneCall },
            Users = users,
            Chats = new TVector<IChat>(),
            Date = DateTime.UtcNow.ToTimestamp(),
            Seq = 0
        };

        // Send to caller (admin) - notify them that call was accepted
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, evt.AdminId),
            updates,
            updatesType: UpdatesType.Updates
        );

        Console.WriteLine($"[DEBUG PhoneCallDomainEventHandler] Update sent to caller AdminId={evt.AdminId}");

        // NOTE: Do NOT send RPC response here - AcceptCallHandler already returned response to callee
        // The RPC response is handled by AcceptCallHandler with proper Users vector
    }

    public async Task HandleAsync(
        IDomainEvent<PhoneCallAggregate, PhoneCallId, PhoneCallConfirmedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;

        // Convert PhoneConnectionInfo to IPhoneConnection
        var connections = new TVector<IPhoneConnection>();
        foreach (var conn in evt.Connections)
        {
            if (conn.IsTurn)
            {
                connections.Add(new TPhoneConnectionWebrtc
                {
                    Id = conn.Id,
                    Ip = conn.Ip,
                    Ipv6 = conn.Ipv6,
                    Port = conn.Port,
                    Turn = true,
                    Stun = conn.IsStun,
                    Username = conn.Username,
                    Password = conn.Password
                });
            }
            else
            {
                connections.Add(new TPhoneConnection
                {
                    Id = conn.Id,
                    Ip = conn.Ip,
                    Ipv6 = conn.Ipv6,
                    Port = conn.Port,
                    PeerTag = new byte[16] // Generate peer tag if needed
                });
            }
        }

        // Send TPhoneCall (confirmed) to the participant (callee)
        var phoneCall = new TPhoneCall
        {
            Id = evt.CallId,
            AccessHash = evt.AccessHash, // Use AccessHash from event
            Date = DateTime.UtcNow.ToTimestamp(),
            AdminId = evt.AdminId,
            ParticipantId = evt.ParticipantId,
            GAOrB = evt.GA.ToReadOnlyMemory(),
            KeyFingerprint = evt.KeyFingerprint,
            Protocol = evt.Protocol.ToPhoneCallProtocol(),
            Connections = connections,
            StartDate = evt.StartDate,
            P2pAllowed = true,
            Video = evt.IsVideo
        };

        var updatePhoneCall = new TUpdatePhoneCall
        {
            PhoneCall = phoneCall
        };

        // Get user info for both participants
        var users = await GetUsersAsync(evt.RequestInfo, evt.AdminId, evt.ParticipantId);

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate> { updatePhoneCall },
            Users = users,
            Chats = new TVector<IChat>(),
            Date = evt.StartDate,
            Seq = 0
        };

        // Send update to callee (participant) - notify them that call is confirmed
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, evt.ParticipantId),
            updates,
            updatesType: UpdatesType.Updates
        );
        
        // NOTE: Do NOT send RPC response here - ConfirmCallHandler already returned response to caller
        // The RPC response is handled by ConfirmCallHandler with proper Users vector
    }

    public async Task HandleAsync(
        IDomainEvent<PhoneCallAggregate, PhoneCallId, PhoneCallDiscardedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;

        // Convert PhoneCallDiscardReason to IPhoneCallDiscardReason
        IPhoneCallDiscardReason? discardReason = evt.Reason switch
        {
            PhoneCallDiscardReason.Missed => new TPhoneCallDiscardReasonMissed(),
            PhoneCallDiscardReason.Disconnected => new TPhoneCallDiscardReasonDisconnect(),
            PhoneCallDiscardReason.Hangup => new TPhoneCallDiscardReasonHangup(),
            PhoneCallDiscardReason.Busy => new TPhoneCallDiscardReasonBusy(),
            _ => null
        };

        var phoneCallDiscarded = new TPhoneCallDiscarded
        {
            Id = evt.CallId,
            Reason = discardReason,
            Duration = evt.Duration,
            NeedRating = false,
            NeedDebug = false,
            Video = evt.IsVideo
        };

        var updatePhoneCall = new TUpdatePhoneCall
        {
            PhoneCall = phoneCallDiscarded
        };

        // Get user info for both participants
        var users = await GetUsersAsync(evt.RequestInfo, evt.AdminId, evt.ParticipantId);

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate> { updatePhoneCall },
            Users = users,
            Chats = new TVector<IChat>(),
            Date = evt.Date,
            Seq = 0
        };

        // Send update to BOTH participants (call is ended)
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, evt.AdminId),
            updates,
            updatesType: UpdatesType.Updates
        );
        
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, evt.ParticipantId),
            updates,
            updatesType: UpdatesType.Updates
        );

        // NOTE: Do NOT send RPC response here - DiscardCallHandler already returned response to caller
        // The RPC response is handled by DiscardCallHandler with proper Updates
    }

    public Task HandleAsync(
        IDomainEvent<PhoneCallAggregate, PhoneCallId, PhoneCallReceivedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        // Just mark as received, no update needed
        return Task.CompletedTask;
    }
    
    private async Task<TVector<IUser>> GetUsersAsync(RequestInfo requestInfo, long userId1, long userId2)
    {
        var userIds = new List<long> { userId1, userId2 };
        var userReadModels = await userAppService.GetListAsync(userIds);
        
        var users = new List<IUser>();
        foreach (var userReadModel in userReadModels)
        {
            var photos = await photoAppService.GetPhotosAsync(userReadModel);
            var user = userConverterService.ToUser(requestInfo, userReadModel, photos, layer: requestInfo.Layer);
            users.Add(user);
        }
        
        return new TVector<IUser>(users);
    }
}
