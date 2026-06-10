namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl;

///<summary>
/// Invokes a query after a successful completion of previous queries.
/// See <a href="https://corefork.telegram.org/method/invokeAfterMsgs" />
///</summary>
internal sealed class InvokeAfterMsgsHandler(
    IInvokeAfterMsgProcessor invokeAfterMsgProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.RequestInvokeAfterMsgs, IObject>,
        IInvokeAfterMsgsHandler
{
    protected override async Task<IObject> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.RequestInvokeAfterMsgs obj)
    {
        // Check if all prerequisite messages have been processed
        var allProcessed = true;
        foreach (var msgId in obj.MsgIds)
        {
            if (!invokeAfterMsgProcessor.ExistsInRecentMessageId(msgId))
            {
                allProcessed = false;
                break;
            }
        }

        if (allProcessed)
        {
            return await invokeAfterMsgProcessor.HandleAsync(input, obj.Query);
        }

        // Queue for later execution after the last prerequisite
        var lastMsgId = obj.MsgIds[^1];
        invokeAfterMsgProcessor.Enqueue(lastMsgId, input, obj.Query);
        return null!;
    }
}
