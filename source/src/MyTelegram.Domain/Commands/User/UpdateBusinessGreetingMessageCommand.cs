using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Commands.User;

public class UpdateBusinessGreetingMessageCommand(UserId aggregateId, BusinessGreetingMessage greetingMessage) 
    : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public BusinessGreetingMessage GreetingMessage { get; } = greetingMessage;
}
