using MyTelegram.Schema;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Handlers.Impl;

/// <summary>
/// Handles destroy_session — destroys the specified session.
/// Reconstructed from the original binary.
/// </summary>
public sealed class DestroySessionHandler : ISessionHandler<RequestDestroySession, IDestroySessionRes>
{
    private readonly ISessionService _sessionService;
    private readonly ILogger<DestroySessionHandler> _logger;

    public DestroySessionHandler(
        ISessionService sessionService,
        ILogger<DestroySessionHandler> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    public async Task<IDestroySessionRes> HandleAsync(IRequestInput input, RequestDestroySession request)
    {
        var sessionId = request.SessionId;
        var removed = await _sessionService
            .RemoveDisconnectedSessionAsync(input.ConnectionId, input.AuthKeyId, sessionId)
            .ConfigureAwait(false);

        if (removed)
        {
            _logger.LogDebug("Session destroyed: sessionId={SessionId} authKey={AuthKeyId}",
                sessionId, input.AuthKeyId);
            return new TDestroySessionOk { SessionId = sessionId };
        }

        _logger.LogDebug("Session not found for destroy: sessionId={SessionId} authKey={AuthKeyId}",
            sessionId, input.AuthKeyId);
        return new TDestroySessionNone { SessionId = sessionId };
    }
}
