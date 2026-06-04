using MyTelegram.Domain.Aggregates.GroupCall;

namespace MyTelegram.Domain.Commands.GroupCall;

public class EditGroupCallTitleCommand(
    GroupCallId aggregateId,
    RequestInfo requestInfo,
    string title)
    : RequestCommand2<GroupCallAggregate, GroupCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public string Title { get; } = title;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return System.Text.Encoding.UTF8.GetBytes(Title);
    }
}
