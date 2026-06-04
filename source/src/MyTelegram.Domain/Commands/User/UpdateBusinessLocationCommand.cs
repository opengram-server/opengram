using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Commands.User;

public class UpdateBusinessLocationCommand(UserId aggregateId, BusinessLocation location) 
    : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public BusinessLocation Location { get; } = location;
}
