using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Commands.StarGift;

public class InitiateStarGiftCommand(StarGiftId aggregateId, RequestInfo requestInfo, long amount)
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long Amount { get; } = amount;
}
