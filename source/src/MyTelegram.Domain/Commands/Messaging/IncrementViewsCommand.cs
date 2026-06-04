namespace MyTelegram.Domain.Commands.Messaging;

public class IncrementViewsCommand(MessageId aggregateId)
    : Command<MessageAggregate, MessageId, IExecutionResult>(aggregateId);