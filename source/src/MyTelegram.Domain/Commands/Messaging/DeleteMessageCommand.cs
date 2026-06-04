namespace MyTelegram.Domain.Commands.Messaging;

public class DeleteMessageCommand(MessageId aggregateId, RequestInfo requestInfo)
    : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>(aggregateId, requestInfo);