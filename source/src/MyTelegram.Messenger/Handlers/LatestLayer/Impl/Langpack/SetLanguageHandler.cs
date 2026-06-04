using MyTelegram.Schema;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Langpack;

/// <summary>
/// Handler for setting user language.
/// Note: RequestSetLanguage schema is currently missing or not identified.
/// This is a placeholder implementation.
/// </summary>
internal sealed class SetLanguageHandler : IObjectHandler // : RpcResultObjectHandler<RequestSetLanguage, IBool>, Payments.ISetLanguageHandler
{
    public Task<IObject> HandleAsync(IRequestInput input, IObject obj)
    {
        throw new NotImplementedException("RequestSetLanguage schema not found.");
    }
}
