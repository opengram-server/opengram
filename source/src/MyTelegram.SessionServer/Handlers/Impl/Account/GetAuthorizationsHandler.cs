using MyTelegram.Schema;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Handlers.Impl.Account;

/// <summary>
/// Handles account.getAuthorizations — returns active sessions.
/// In the SessionServer context this typically gets dispatched to the Messenger server,
/// but the handler is registered here per the original binary structure.
/// Reconstructed from the original binary.
/// </summary>
public sealed class GetAuthorizationsHandler
{
    // This handler is mostly pass-through to the Messenger server.
    // The SessionServer doesn't handle it locally — it's registered in the
    // handler map so the dispatcher knows it exists.
}
