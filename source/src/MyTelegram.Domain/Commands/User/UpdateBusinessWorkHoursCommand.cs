using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Commands.User;

public class UpdateBusinessWorkHoursCommand(UserId aggregateId, BusinessWorkHours workHours) 
    : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public BusinessWorkHours WorkHours { get; } = workHours;
}
