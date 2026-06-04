using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Commands.User;

public class CreateBusinessChatLinkCommand(UserId aggregateId, BusinessChatLink chatLink) 
    : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public BusinessChatLink ChatLink { get; } = chatLink;
}
