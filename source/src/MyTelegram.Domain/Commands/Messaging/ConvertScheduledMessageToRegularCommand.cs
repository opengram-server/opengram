namespace MyTelegram.Domain.Commands.Messaging;

/// <summary>
/// Command to convert a scheduled message to a regular message (send it now)
/// </summary>
public class ConvertScheduledMessageToRegularCommand(
    MessageId aggregateId,
    RequestInfo requestInfo)
    : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>(aggregateId, requestInfo);
