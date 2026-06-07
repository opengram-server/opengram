using MyTelegram.Core;
using MyTelegram.EventBus;
using MyTelegram.Schema;
using MyTelegram.SessionServer.Caching;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Handlers.Impl.Auth;

/// <summary>
/// Handles auth.logOut — logs out the user from the current session.
/// Reconstructed from the original binary.
/// </summary>
public sealed class LogOutHandler : ISessionHandler<Schema.Auth.RequestLogOut, IObject>
{
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly ISessionService _sessionService;
    private readonly IOnlineUserHelper _onlineUserHelper;
    private readonly IEventBus _eventBus;
    private readonly ILogger<LogOutHandler> _logger;

    public LogOutHandler(
        IAuthKeyHelper authKeyHelper,
        ISessionService sessionService,
        IOnlineUserHelper onlineUserHelper,
        IEventBus eventBus,
        ILogger<LogOutHandler> logger)
    {
        _authKeyHelper = authKeyHelper;
        _sessionService = sessionService;
        _onlineUserHelper = onlineUserHelper;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<IObject> HandleAsync(IRequestInput input, Schema.Auth.RequestLogOut request)
    {
        var authKeyId = input.AuthKeyId;
        var session = _sessionService.GetSession(authKeyId);
        var userId = session?.UserId ?? 0;
        var permAuthKeyId = session?.PermAuthKeyId ?? 0;

        // Deactivate and remove auth key
        _sessionService.Deactivate(authKeyId, revoked: false);
        _authKeyHelper.RemoveAuthKeyItem(authKeyId);

        if (userId > 0)
        {
            _onlineUserHelper.SetOffline(userId, authKeyId);

            // Notify the Messenger server about logout
            await _eventBus.PublishAsync(new UnRegisterAuthKeyEvent(permAuthKeyId, userId))
                .ConfigureAwait(false);
        }

        _logger.LogInformation("Auth.LogOut: authKey={AuthKeyId} userId={UserId}", authKeyId, userId);

        // Return auth.loggedOut
        return new Schema.Auth.TLoggedOut();
    }
}
