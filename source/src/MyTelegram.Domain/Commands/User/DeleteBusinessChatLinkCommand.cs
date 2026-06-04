namespace MyTelegram.Domain.Commands.User;

public class DeleteBusinessChatLinkCommand(UserId aggregateId, string linkId) 
    : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public string LinkId { get; } = linkId;
}
