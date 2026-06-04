using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Commands.GroupCall;

public class UpdateGroupCallSettingsCommand(
    GroupCallId aggregateId,
    RequestInfo requestInfo,
    bool? joinMuted)
    : RequestCommand2<GroupCallAggregate, GroupCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool? JoinMuted { get; } = joinMuted;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(joinMuted?.GetHashCode() ?? 0);
    }
}
