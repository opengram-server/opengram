using System.Diagnostics;

namespace MyTelegram.AuthServer.Handlers;

public class ReqPqMultiHandler(
    IStep1Helper step1ServerHelper,
    ILogger<ReqPqMultiHandler> logger,
    ICacheManager<AuthCacheItem> cacheManager
) : BaseObjectHandler<RequestReqPqMulti, IResPQ>, IReqPqMultiHandler
{
    protected override async Task<IResPQ> HandleCoreAsync(
        IRequestInput input,
        RequestReqPqMulti obj
    )
    {
        var sw = Stopwatch.StartNew();
        var dto = step1ServerHelper.GetResponse(obj.Nonce);

        var authCacheItem = new AuthCacheItem(obj.Nonce, dto.ServerNonce, dto.P, dto.Q, false);
        var key = AuthCacheItem.GetCacheKey(dto.ServerNonce);
        await cacheManager.SetAsync(
            key,
            authCacheItem,
            MyTelegramConsts.AuthKeyExpireSeconds
        );
        sw.Stop();
        logger.LogInformation(
            "[Step1] ReqPqMultiHandler, connectionId={ConnectionId}, nonce: {Nonce} reqMsgId: {ReqMsgId}, authKeyId: {AuthKeyId} {TimeSpan}ms",
            input.ConnectionId,
            obj.Nonce.ToHexString(),
            input.ReqMsgId,
            input.AuthKeyId,
            sw.Elapsed.TotalMilliseconds
        );

        return dto.ResPq;
    }
}