namespace MyTelegram.Domain.Commands.Channel;

public class
    IncrementParticipantCountCommand(ChannelId aggregateId)
    : Command<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId) //, IHasCorrelationId
{
    /*
        public IncrementParticipantCountCommand(ChannelId aggregateId,
            ISourceId sourceId) : base(aggregateId, sourceId)
        {
        }
*/
}