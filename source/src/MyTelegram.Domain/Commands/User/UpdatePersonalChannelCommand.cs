namespace MyTelegram.Domain.Commands.User;

public class UpdatePersonalChannelCommand(UserId aggregateId, RequestInfo requestInfo, long? personalChannelId)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public long? PersonalChannelId { get; } = personalChannelId;
}