namespace MyTelegram.AuthServer.Handlers;

public class ReqDhParamsHandler(IStep2Helper step2ServerHelper, ILogger<ReqDhParamsHandler> logger)
    : BaseObjectHandler<RequestReqDHParams, IServerDHParams>,
        IReqDhParamsHandler
{
    protected override async Task<IServerDHParams> HandleCoreAsync(
        IRequestInput input,
        RequestReqDHParams obj
    )
    {
        var dto = await step2ServerHelper.GetServerDhParamsAsync(obj);
        logger.LogInformation(
            "[Step2] ReqDhParamsHandler, connectionId: {ConnectionId}, reqMsgId: {ReqMsgId}",
            input.ConnectionId,
            input.ReqMsgId
        );

        return dto.ServerDhParams;
    }
}