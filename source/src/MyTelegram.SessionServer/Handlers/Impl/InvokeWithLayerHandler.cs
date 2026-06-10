using MyTelegram.Schema;
using MyTelegram.SessionServer.Caching;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Handlers.Impl;

/// <summary>
/// Handles invokeWithLayer — sets the client's layer and re-dispatches the inner query.
/// Reconstructed from the original binary.
/// </summary>
public sealed class InvokeWithLayerHandler : IUnwrappingSessionHandler<RequestInvokeWithLayer>
{
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly ISessionService _sessionService;
    private readonly ILogger<InvokeWithLayerHandler> _logger;

    public InvokeWithLayerHandler(
        IAuthKeyHelper authKeyHelper,
        ISessionService sessionService,
        ILogger<InvokeWithLayerHandler> logger)
    {
        _authKeyHelper = authKeyHelper;
        _sessionService = sessionService;
        _logger = logger;
    }

    public async Task HandleAsync(IRequestInput input, RequestInvokeWithLayer request,
        Func<IRequestInput, IObject, Task> dispatch)
    {
        var layer = request.Layer;

        // Update the session layer
        var session = _sessionService.GetSession(input.AuthKeyId);
        if (session != null)
        {
            session.Layer = layer;
        }

        // Update the auth key item layer
        _authKeyHelper.UpdateLayer(input.AuthKeyId, layer);

        _logger.LogDebug("InvokeWithLayer: authKey={AuthKeyId} layer={Layer}", input.AuthKeyId, layer);

        // Create updated input with new layer
        if (input is SessionRequestInput sri)
        {
            sri.Layer = layer;
        }

        // Re-dispatch the inner query
        if (request.Query != null)
        {
            await dispatch(input, request.Query).ConfigureAwait(false);
        }
    }
}
