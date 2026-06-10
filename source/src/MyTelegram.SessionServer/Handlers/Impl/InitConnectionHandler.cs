using MyTelegram.Schema;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Handlers.Impl;

/// <summary>
/// Handles initConnection — records the client app info and re-dispatches the inner query.
/// Reconstructed from the original binary. This is typically wrapped inside invokeWithLayer.
/// </summary>
public sealed class InitConnectionHandler : IUnwrappingSessionHandler<RequestInitConnection>
{
    private readonly ISessionService _sessionService;
    private readonly ILogger<InitConnectionHandler> _logger;

    public InitConnectionHandler(
        ISessionService sessionService,
        ILogger<InitConnectionHandler> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    public async Task HandleAsync(IRequestInput input, RequestInitConnection request,
        Func<IRequestInput, IObject, Task> dispatch)
    {
        // Record the connection info in the session
        var session = _sessionService.GetSession(input.AuthKeyId);
        if (session != null)
        {
            session.ApiId = request.ApiId;
            session.AppName = $"{request.DeviceModel} {request.SystemVersion}";
            session.DeviceModel = request.DeviceModel;
            session.SystemVersion = request.SystemVersion;
            session.LangCode = request.LangCode;
            session.LangPack = request.LangPack;
            session.SystemLangCode = request.SystemLangCode;
        }

        _logger.LogDebug(
            "InitConnection: authKey={AuthKeyId} apiId={ApiId} device={Device} sys={System} lang={Lang}",
            input.AuthKeyId, request.ApiId, request.DeviceModel, request.SystemVersion, request.LangCode);

        // Re-dispatch the inner query
        if (request.Query != null)
        {
            await dispatch(input, request.Query).ConfigureAwait(false);
        }
    }
}
