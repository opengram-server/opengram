using MyTelegram.Schema;
using MyTelegram.SessionServer.Caching;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Handlers.Impl;

/// <summary>
/// Handles destroy_auth_key — removes an auth key and deactivates all its sessions.
/// Reconstructed from the original binary.
/// </summary>
public sealed class DestroyAuthKeyHandler : ISessionHandler<RequestDestroyAuthKey, IDestroyAuthKeyRes>
{
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly ISessionService _sessionService;
    private readonly ILogger<DestroyAuthKeyHandler> _logger;

    public DestroyAuthKeyHandler(
        IAuthKeyHelper authKeyHelper,
        ISessionService sessionService,
        ILogger<DestroyAuthKeyHandler> logger)
    {
        _authKeyHelper = authKeyHelper;
        _sessionService = sessionService;
        _logger = logger;
    }

    public Task<IDestroyAuthKeyRes> HandleAsync(IRequestInput input, RequestDestroyAuthKey request)
    {
        var authKeyId = input.AuthKeyId;
        var item = _authKeyHelper.GetAuthKeyItem(authKeyId);

        if (item == null)
        {
            _logger.LogDebug("Auth key not found for destroy: authKey={AuthKeyId}", authKeyId);
            return Task.FromResult<IDestroyAuthKeyRes>(new TDestroyAuthKeyNone());
        }

        // Deactivate all sessions for this auth key
        _sessionService.Deactivate(authKeyId, revoked: false);
        _authKeyHelper.RemoveAuthKeyItem(authKeyId);

        _logger.LogInformation("Auth key destroyed: authKey={AuthKeyId}", authKeyId);
        return Task.FromResult<IDestroyAuthKeyRes>(new TDestroyAuthKeyOk());
    }
}
