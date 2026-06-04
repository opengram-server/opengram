namespace MyTelegram.Domain.Commands.Channel;

public class EditChannelAboutCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    long selfUserId,
    string? about)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public string? About { get; } = about;
    public long SelfUserId { get; } = selfUserId;
}