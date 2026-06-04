using MyTelegram.Schema.Account;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Changes the main profile tab of the current user
/// See <a href="https://corefork.telegram.org/method/account.setMainProfileTab" />
///</summary>
internal sealed class SetMainProfileTabHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor,
    ILogger<SetMainProfileTabHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestSetMainProfileTab, IBool>
{
    protected override async Task<IBool> HandleCoreAsync(
        IRequestInput input,
        RequestSetMainProfileTab obj)
    {
        logger.LogInformation(
            "Setting main profile tab for user {UserId}: {TabType}",
            input.UserId, obj.Tab.GetType().Name);

        // For now, just return success
        // In a full implementation, this would save the user's preferred tab
        
        return new TBoolTrue();
    }
}
