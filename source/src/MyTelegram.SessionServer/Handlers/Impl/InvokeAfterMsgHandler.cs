using MyTelegram.Schema;

namespace MyTelegram.SessionServer.Handlers.Impl;

/// <summary>
/// Handles invokeAfterMsg — the query is processed after the specified message has been processed.
/// In the SessionServer, we simply re-dispatch the inner query (ordering is handled by the pipeline).
/// Reconstructed from the original binary.
/// </summary>
public sealed class InvokeAfterMsgHandler : IUnwrappingSessionHandler<RequestInvokeAfterMsg>
{
    private readonly ILogger<InvokeAfterMsgHandler> _logger;

    public InvokeAfterMsgHandler(ILogger<InvokeAfterMsgHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(IRequestInput input, RequestInvokeAfterMsg request,
        Func<IRequestInput, IObject, Task> dispatch)
    {
        _logger.LogDebug("InvokeAfterMsg: afterMsgId={AfterMsgId} authKey={AuthKeyId}",
            request.MsgId, input.AuthKeyId);

        // Re-dispatch the inner query — ordering guarantees are not enforced at the
        // session-server level; the Messenger server handles them.
        if (request.Query != null)
        {
            await dispatch(input, request.Query).ConfigureAwait(false);
        }
    }
}
