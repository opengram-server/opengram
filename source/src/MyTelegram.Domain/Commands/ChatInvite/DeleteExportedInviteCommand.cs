namespace MyTelegram.Domain.Commands.ChatInvite;

public class DeleteExportedInviteCommand(ChatInviteId aggregateId, RequestInfo requestInfo)
    : RequestCommand2<ChatInviteAggregate, ChatInviteId, IExecutionResult>(aggregateId, requestInfo);