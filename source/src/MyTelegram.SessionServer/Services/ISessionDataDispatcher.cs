using MyTelegram.Core;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Routes deserialized RPC requests to the appropriate downstream server
/// (Command/Query/Sticker server) based on ObjectId classification.
/// Reconstructed from the original binary's SessionDataDispatcher.
/// </summary>
public interface ISessionDataDispatcher
{
    Task DispatchAsync(InternalSessionData sessionData);
}
