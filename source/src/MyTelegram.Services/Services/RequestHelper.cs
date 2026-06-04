using System.Collections.Concurrent;

namespace MyTelegram.Services.Services;

public class RequestHelper(
    IScheduleAppService scheduleAppService,
    IEventBus eventBus
) : IRequestHelper, ISingletonDependency
{
    private readonly ConcurrentDictionary<long, byte> _requestMessageIds = [];
    private readonly int _duplicateRequestIntervalSeconds = 300;
    /// <summary>
    ///     Check for duplicate requests within 300 seconds
    /// </summary>
    /// <param name="requestInfo"></param>
    /// <returns></returns>
    public async Task<bool> CheckRequestAsync(IRequestInput requestInfo)
    {
        var key = requestInfo.ReqMsgId;
        if (_requestMessageIds.ContainsKey(key))
        {
            await eventBus.PublishAsync(new DuplicateCommandEvent(requestInfo.PermAuthKeyId, requestInfo.UserId,
                requestInfo.ReqMsgId));

            return false;
        }

        if (_requestMessageIds.TryAdd(key, 0))
        {
            scheduleAppService.Execute(() =>
                {
                    _requestMessageIds.TryRemove(key, out _);
                },
                TimeSpan.FromSeconds(_duplicateRequestIntervalSeconds));

            return true;
        }

        await eventBus.PublishAsync(new DuplicateCommandEvent(requestInfo.PermAuthKeyId, requestInfo.UserId,
            requestInfo.ReqMsgId));

        return false;
    }
}