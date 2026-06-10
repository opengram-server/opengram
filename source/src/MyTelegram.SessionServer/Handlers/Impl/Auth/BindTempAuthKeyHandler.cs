using MyTelegram.Schema;
using MyTelegram.Schema.Auth;
using MyTelegram.SessionServer.Caching;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Handlers.Impl.Auth;

/// <summary>
/// Handles auth.bindTempAuthKey — binds a temporary auth key to a permanent one.
/// Reconstructed from the original binary.
/// </summary>
public sealed class BindTempAuthKeyHandler : ISessionHandler<RequestBindTempAuthKey, IObject>
{
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly ISessionService _sessionService;
    private readonly ILogger<BindTempAuthKeyHandler> _logger;

    public BindTempAuthKeyHandler(
        IAuthKeyHelper authKeyHelper,
        ISessionService sessionService,
        ILogger<BindTempAuthKeyHandler> logger)
    {
        _authKeyHelper = authKeyHelper;
        _sessionService = sessionService;
        _logger = logger;
    }

    public async Task<IObject> HandleAsync(IRequestInput input, RequestBindTempAuthKey request)
    {
        var tempAuthKeyId = input.AuthKeyId;
        var permAuthKeyId = request.PermAuthKeyId;

        // Bind the temp key to the perm key
        _authKeyHelper.BindTempAuthKey(tempAuthKeyId, permAuthKeyId);

        // Get the user from the perm auth key and propagate to temp
        var permItem = _authKeyHelper.GetAuthKeyItem(permAuthKeyId);
        if (permItem?.UserId > 0)
        {
            _authKeyHelper.UpdateUserId(tempAuthKeyId, permItem.UserId);
            await _sessionService.BindUserIdToSessionAsync(tempAuthKeyId, permItem.UserId, 0)
                .ConfigureAwait(false);
        }

        _logger.LogInformation(
            "BindTempAuthKey: temp={TempAuthKeyId} → perm={PermAuthKeyId}",
            tempAuthKeyId, permAuthKeyId);

        return new TBoolTrue();
    }
}
