namespace MyTelegram.Domain.Commands.Messaging;

public class UpdateMessageRelyCommand(MessageId aggregateId, int pts)
    : Command<MessageAggregate, MessageId, IExecutionResult>(aggregateId)
{
    public int Pts { get; } = pts;
}