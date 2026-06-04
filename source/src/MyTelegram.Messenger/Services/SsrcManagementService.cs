using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.QueryHandlers.MongoDB;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Messenger.Services;

public interface ISsrcManagementService
{
    Task<int> GetOrCreateSsrcAsync(long userId, long callId);
    Task ReleaseSsrcAsync(long userId, long callId);
}

public class SsrcManagementService : ISsrcManagementService
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly Random _random = Random.Shared;

    public SsrcManagementService(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    public async Task<int> GetOrCreateSsrcAsync(long userId, long callId)
    {
        var groupCall = await _queryProcessor.ProcessAsync(
            new GetGroupCallByIdQuery(GroupCallId.Create(callId).Value),
            default);

        if (groupCall != null)
        {
            var existingParticipant = groupCall.Participants
                .FirstOrDefault(p => p.PeerId == userId && !p.Left);

            if (existingParticipant != null && existingParticipant.Source > 0)
            {
                return existingParticipant.Source;
            }
        }

        return GenerateUniqueSsrc(groupCall);
    }

    public Task ReleaseSsrcAsync(long userId, long callId)
    {
        return Task.CompletedTask;
    }

    private int GenerateUniqueSsrc(IGroupCallReadModel? groupCall)
    {
        const int minSsrc = 1000000;
        const int maxSsrc = 9999999;
        const int maxAttempts = 100;

        var usedSsrcs = groupCall?.Participants
            .Where(p => !p.Left && p.Source > 0)
            .Select(p => p.Source)
            .ToHashSet() ?? new HashSet<int>();

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var ssrc = _random.Next(minSsrc, maxSsrc + 1);
            
            if (!usedSsrcs.Contains(ssrc))
            {
                return ssrc;
            }
        }

        for (int ssrc = minSsrc; ssrc <= maxSsrc; ssrc++)
        {
            if (!usedSsrcs.Contains(ssrc))
            {
                return ssrc;
            }
        }

        throw new InvalidOperationException("Unable to generate unique SSRC. All SSRCs in range are used.");
    }
}
