using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Commands.User;

public class UpdateBusinessAwayMessageCommand(UserId aggregateId, BusinessAwayMessage awayMessage) 
    : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public BusinessAwayMessage AwayMessage { get; } = awayMessage;
}
