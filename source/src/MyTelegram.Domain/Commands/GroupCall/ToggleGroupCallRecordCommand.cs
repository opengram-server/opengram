using MyTelegram.Domain.Aggregates.GroupCall;

namespace MyTelegram.Domain.Commands.GroupCall;

public class ToggleGroupCallRecordCommand(
    GroupCallId aggregateId,
    RequestInfo requestInfo,
    bool start,
    bool video,
    string? title,
    bool? videoPortrait)
    : RequestCommand2<GroupCallAggregate, GroupCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool Start { get; } = start;
    public bool Video { get; } = video;
    public string? Title { get; } = title;
    public bool? VideoPortrait { get; } = videoPortrait;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(Start);
    }
}
