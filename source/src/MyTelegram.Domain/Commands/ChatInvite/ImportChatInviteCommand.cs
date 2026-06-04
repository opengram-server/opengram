namespace MyTelegram.Domain.Commands.ChatInvite;

public class ImportChatInviteCommand(
    ChatInviteId aggregateId,
    RequestInfo requestInfo,
    ChatInviteRequestState chatInviteRequestState,
    int date)
    : RequestCommand2<ChatInviteAggregate, ChatInviteId, IExecutionResult>(aggregateId, requestInfo)
{
    public ChatInviteRequestState ChatInviteRequestState { get; } = chatInviteRequestState;
    public int Date { get; } = date;
}