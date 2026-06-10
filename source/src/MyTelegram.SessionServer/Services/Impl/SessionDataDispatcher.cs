using Microsoft.Extensions.Options;
using MyTelegram.Core;
using MyTelegram.EventBus;
using MyTelegram.SessionServer.Options;

namespace MyTelegram.SessionServer.Services.Impl;

/// <summary>
/// Reconstructed from the original binary's SessionDataDispatcher.
/// Routes deserialized RPC requests to the correct downstream server
/// by publishing typed DataReceivedEvent subtypes via the event bus.
///
/// Routing logic (from ObjectIdConsts):
///   - ObjectId in CommandServerHandlers → MessengerCommandDataReceivedEvent
///   - ObjectId in StickerServerObjectIds → StickerDataReceivedEvent
///   - Otherwise → MessengerQueryDataReceivedEvent (default)
/// </summary>
public sealed class SessionDataDispatcher : ISessionDataDispatcher
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<SessionDataDispatcher> _logger;
    private readonly IOptionsMonitor<MyTelegramSessionServerOptions> _options;

    public SessionDataDispatcher(
        IEventBus eventBus,
        ILogger<SessionDataDispatcher> logger,
        IOptionsMonitor<MyTelegramSessionServerOptions> options)
    {
        _eventBus = eventBus;
        _logger = logger;
        _options = options;
    }

    public async Task DispatchAsync(InternalSessionData sessionData)
    {
        var objectId = sessionData.ObjectId;
        var input = sessionData.RequestInput;

        _logger.LogDebug(
            "Dispatching objectId=0x{ObjectId:X8} user={UserId} authKey={AuthKeyId}",
            objectId, input.UserId, input.AuthKeyId);

        if (ObjectIdConsts.CommandServerHandlers.ContainsKey(objectId))
        {
            // Route to command server
            var evt = MessengerCommandDataReceivedEvent.Create();
            evt.ConnectionId = input.ConnectionId;
            evt.RequestId = input.RequestId;
            evt.ObjectId = objectId;
            evt.UserId = input.UserId;
            evt.ReqMsgId = input.ReqMsgId;
            evt.SeqNumber = input.SeqNumber;
            evt.AuthKeyId = input.AuthKeyId;
            evt.PermAuthKeyId = input.PermAuthKeyId;
            evt.Layer = input.Layer;
            evt.Date = input.Date;
            evt.DeviceType = input.DeviceType;
            evt.ClientIp = input.ClientIp;
            evt.SessionId = input.SessionId;
            evt.AccessHashKeyId = input.AccessHashKeyId;
            // Data is the serialized bytes of the request — for gRPC transport
            // the Messenger server deserializes from the event data field.
            // In the local pipeline, RequestData is already deserialized.
            await _eventBus.PublishAsync(evt).ConfigureAwait(false);
        }
        else if (IsStickerObjectId(objectId))
        {
            var evt = StickerDataReceivedEvent.Create();
            CopyInputToEvent(input, evt, objectId);
            await _eventBus.PublishAsync(evt).ConfigureAwait(false);
        }
        else
        {
            // Default: query server
            var evt = MessengerQueryDataReceivedEvent.Create();
            CopyInputToEvent(input, evt, objectId);
            await _eventBus.PublishAsync(evt).ConfigureAwait(false);
        }
    }

    private bool IsStickerObjectId(uint objectId)
    {
        var opts = _options.CurrentValue;
        return opts.StickerServerObjectIds.Contains(objectId);
    }

    private static void CopyInputToEvent(IRequestInput input, DataReceivedEvent evt, uint objectId)
    {
        evt.ConnectionId = input.ConnectionId;
        evt.RequestId = input.RequestId;
        evt.ObjectId = objectId;
        evt.UserId = input.UserId;
        evt.ReqMsgId = input.ReqMsgId;
        evt.SeqNumber = input.SeqNumber;
        evt.AuthKeyId = input.AuthKeyId;
        evt.PermAuthKeyId = input.PermAuthKeyId;
        evt.Layer = input.Layer;
        evt.Date = input.Date;
        evt.DeviceType = input.DeviceType;
        evt.ClientIp = input.ClientIp;
        evt.SessionId = input.SessionId;
        evt.AccessHashKeyId = input.AccessHashKeyId;
    }
}
