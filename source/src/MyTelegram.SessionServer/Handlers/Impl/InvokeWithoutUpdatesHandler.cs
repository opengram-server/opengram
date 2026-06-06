using MyTelegram.Schema;

namespace MyTelegram.SessionServer.Handlers.Impl;

/// <summary>
/// Handles invokeWithoutUpdates — executes the query without receiving updates.
/// The SessionServer simply re-dispatches (updates suppression is Messenger-level).
/// Reconstructed from the original binary.
/// </summary>
public sealed class InvokeWithoutUpdatesHandler : IUnwrappingSessionHandler<RequestInvokeWithoutUpdates>
{
    private readonly ILogger<InvokeWithoutUpdatesHandler> _logger;

    public InvokeWithoutUpdatesHandler(ILogger<InvokeWithoutUpdatesHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(IRequestInput input, RequestInvokeWithoutUpdates request,
        Func<IRequestInput, IObject, Task> dispatch)
    {
        _logger.LogDebug("InvokeWithoutUpdates: authKey={AuthKeyId}", input.AuthKeyId);

        if (request.Query != null)
        {
            await dispatch(input, request.Query).ConfigureAwait(false);
        }
    }
}
