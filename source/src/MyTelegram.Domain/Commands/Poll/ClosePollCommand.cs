namespace MyTelegram.Domain.Commands.Poll;

public class ClosePollCommand(PollId aggregateId, int closeDate)
    : Command<PollAggregate, PollId, IExecutionResult>(aggregateId)
{
    public int CloseDate { get; } = closeDate;
}
