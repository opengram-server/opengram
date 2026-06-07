using MyTelegram.Schema;

namespace MyTelegram.SessionServer.Handlers.Impl;

/// <summary>
/// Handles msgs_state_req — client asks for the status of messages it has sent.
/// We return all states as "acknowledged" (state=4) since the session server
/// has already processed them. Reconstructed from the original binary.
/// </summary>
public sealed class MsgsStateReqHandler : ISessionHandler<TMsgsStateReq, IObject>
{
    private readonly ILogger<MsgsStateReqHandler> _logger;

    public MsgsStateReqHandler(ILogger<MsgsStateReqHandler> logger)
    {
        _logger = logger;
    }

    public Task<IObject> HandleAsync(IRequestInput input, TMsgsStateReq request)
    {
        // Each byte in Info represents the state of the corresponding msg_id:
        // 1 = unknown, 2 = not received, 3 = not yet acknowledged,
        // 4 = acknowledged, +8 = already delivered
        var count = request.MsgIds?.Count ?? 0;
        var info = new byte[count];
        for (var i = 0; i < count; i++)
        {
            info[i] = 4; // acknowledged
        }

        var result = new TMsgsStateInfo
        {
            ReqMsgId = input.ReqMsgId,
            Info = Convert.ToBase64String(info)
        };

        _logger.LogDebug("MsgsStateReq: {Count} messages, authKey={AuthKeyId}",
            count, input.AuthKeyId);

        return Task.FromResult<IObject>(result);
    }
}
