using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.Commands.StarGift;

public class UpgradeStarGiftCommand(StarGiftId aggregateId, RequestInfo requestInfo, int upgradeMsgId, int date)
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public int UpgradeMsgId { get; } = upgradeMsgId;
    public int Date { get; } = date;
}
