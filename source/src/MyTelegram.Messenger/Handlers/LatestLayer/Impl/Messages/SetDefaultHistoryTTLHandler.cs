// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Changes the default value of the Time-To-Live setting, applied to all new chats.
/// See <a href="https://corefork.telegram.org/method/messages.setDefaultHistoryTTL" />
///</summary>
internal sealed class SetDefaultHistoryTTLHandler(
    ICommandBus commandBus) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSetDefaultHistoryTTL, IBool>,
    Messages.ISetDefaultHistoryTTLHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSetDefaultHistoryTTL obj)
    {
        // Validate period (max 1 year)
        if (obj.Period < 0 || obj.Period > 31536000)
        {
            RpcErrors.RpcErrors400.TtlPeriodInvalid.ThrowRpcError();
        }
        
        var command = new SetUserDefaultHistoryTTLCommand(
            UserId.Create(input.UserId),
            input.ToRequestInfo(),
            obj.Period);
            
        await commandBus.PublishAsync(command, CancellationToken.None);
        
        return new TBoolTrue();
    }
}
