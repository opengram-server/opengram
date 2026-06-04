namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Gets the default value of the Time-To-Live setting, applied to all new chats.
/// See <a href="https://corefork.telegram.org/method/messages.getDefaultHistoryTTL" />
///</summary>
internal sealed class GetDefaultHistoryTTLHandler(
    IUserAppService userAppService) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetDefaultHistoryTTL, MyTelegram.Schema.IDefaultHistoryTTL>,
    Messages.IGetDefaultHistoryTTLHandler
{
    protected override async Task<MyTelegram.Schema.IDefaultHistoryTTL> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetDefaultHistoryTTL obj)
    {
        var userReadModel = await userAppService.GetAsync(input.UserId);
        
        return new TDefaultHistoryTTL
        {
            Period = userReadModel.DefaultHistoryTTL ?? 0
        };
    }
}
