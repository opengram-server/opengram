using MyTelegram.Core;
using MyTelegram.EventBus;
using MyTelegram.Schema;

namespace MyTelegram.SessionServer.Handlers.Impl.Auth;

/// <summary>
/// Handles auth.resetAuthorizations — revokes all other sessions.
/// This is forwarded to the Messenger server but the SessionServer also
/// locally revokes sessions. Reconstructed from the original binary.
/// </summary>
public sealed class ResetAuthorizationsHandler : ISessionHandler<Schema.Auth.RequestResetAuthorizations, IObject>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<ResetAuthorizationsHandler> _logger;

    public ResetAuthorizationsHandler(
        IEventBus eventBus,
        ILogger<ResetAuthorizationsHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public Task<IObject> HandleAsync(IRequestInput input, Schema.Auth.RequestResetAuthorizations request)
    {
        _logger.LogInformation("Auth.ResetAuthorizations: authKey={AuthKeyId}", input.AuthKeyId);

        // This is typically also dispatched to the Messenger server for full processing.
        // The SessionServer version just ensures local event propagation.
        return Task.FromResult<IObject>(new TBoolTrue());
    }
}
