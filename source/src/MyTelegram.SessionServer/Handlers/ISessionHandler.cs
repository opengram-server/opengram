using MyTelegram.Schema;

namespace MyTelegram.SessionServer.Handlers;

/// <summary>
/// Base interface for session-server-local MTProto handlers.
/// These handlers process requests that the SessionServer resolves locally
/// without forwarding to the Messenger server.
/// </summary>
public interface ISessionHandler<in TRequest, TResponse>
    where TRequest : IObject
    where TResponse : IObject
{
    Task<TResponse> HandleAsync(IRequestInput input, TRequest request);
}

/// <summary>
/// Marker interface for handlers that unwrap their inner query (InvokeWithLayer, InitConnection, etc.)
/// and re-dispatch it to the pipeline.
/// </summary>
public interface IUnwrappingSessionHandler<in TRequest> where TRequest : IObject
{
    Task HandleAsync(IRequestInput input, TRequest request, Func<IRequestInput, IObject, Task> dispatch);
}
