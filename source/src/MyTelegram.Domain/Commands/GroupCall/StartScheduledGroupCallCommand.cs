using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Commands.GroupCall;

public class StartScheduledGroupCallCommand(
    GroupCallId aggregateId,
    RequestInfo requestInfo,
    int date)
    : RequestCommand2<GroupCallAggregate, GroupCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public int Date { get; } = date;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(Date);
    }
}
