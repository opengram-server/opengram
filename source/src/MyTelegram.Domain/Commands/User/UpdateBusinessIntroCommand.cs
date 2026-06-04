using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Commands.User;

public class UpdateBusinessIntroCommand(UserId aggregateId, BusinessIntro businessIntro) 
    : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public BusinessIntro BusinessIntro { get; } = businessIntro;
}
