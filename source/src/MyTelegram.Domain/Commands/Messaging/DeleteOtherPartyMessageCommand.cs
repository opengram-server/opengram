namespace MyTelegram.Domain.Commands.Messaging;

public class DeleteOtherPartyMessageCommand(MessageId aggregateId, RequestInfo requestInfo)
    : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>(aggregateId, requestInfo)
{
    //Revoke = revoke;
}