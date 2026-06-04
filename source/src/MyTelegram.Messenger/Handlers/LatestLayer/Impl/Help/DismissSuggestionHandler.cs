namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Help;

///<summary>
/// Dismiss a <a href="https://corefork.telegram.org/api/config#suggestions">suggestion, see here for more info »</a>.
/// See <a href="https://corefork.telegram.org/method/help.dismissSuggestion" />
///</summary>
internal sealed class DismissSuggestionHandler : RpcResultObjectHandler<RequestDismissSuggestion, IBool>,
    IDismissSuggestionHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        RequestDismissSuggestion obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
