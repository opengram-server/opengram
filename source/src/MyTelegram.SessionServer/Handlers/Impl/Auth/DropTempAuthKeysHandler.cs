using MyTelegram.Schema;
using MyTelegram.SessionServer.Caching;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Handlers.Impl.Auth;

/// <summary>
/// Handles auth.dropTempAuthKeys — drops all temp auth keys except the specified ones.
/// Reconstructed from the original binary.
/// </summary>
public sealed class DropTempAuthKeysHandler : ISessionHandler<Schema.Auth.RequestDropTempAuthKeys, IObject>
{
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly ISessionService _sessionService;
    private readonly ILogger<DropTempAuthKeysHandler> _logger;

    public DropTempAuthKeysHandler(
        IAuthKeyHelper authKeyHelper,
        ISessionService sessionService,
        ILogger<DropTempAuthKeysHandler> logger)
    {
        _authKeyHelper = authKeyHelper;
        _sessionService = sessionService;
        _logger = logger;
    }

    public Task<IObject> HandleAsync(IRequestInput input, Schema.Auth.RequestDropTempAuthKeys request)
    {
        var exceptIds = new HashSet<long>(request.ExceptAuthKeys ?? []);

        // Drop all temp auth keys for this user except the specified ones
        var allKeys = _authKeyHelper.GetAllTempAuthKeyIds();
        var dropped = 0;
        foreach (var tempKeyId in allKeys)
        {
            if (!exceptIds.Contains(tempKeyId))
            {
                var item = _authKeyHelper.GetAuthKeyItem(tempKeyId);
                if (item is { TempAuthKeyId: > 0 } && item.PermAuthKeyId == input.PermAuthKeyId)
                {
                    _authKeyHelper.RemoveAuthKeyItem(tempKeyId);
                    _sessionService.Deactivate(tempKeyId, revoked: false);
                    dropped++;
                }
            }
        }

        _logger.LogInformation("DropTempAuthKeys: dropped {Count} temp keys for permAuth={PermAuthKeyId}",
            dropped, input.PermAuthKeyId);

        return Task.FromResult<IObject>(new TBoolTrue());
    }
}
